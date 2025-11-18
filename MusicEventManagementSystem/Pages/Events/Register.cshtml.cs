using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly IMusicEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MusicDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IMusicEventRepository eventRepository,
            UserManager<ApplicationUser> userManager,
            MusicDbContext context,
            ILogger<RegisterModel> logger)
        {
            _eventRepository = eventRepository;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public MusicEvent MusicEvent { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public int EventID { get; set; }

            [Required(ErrorMessage = "Please select a ticket type.")]
            public int? PricingTierID { get; set; }

            [Required]
            [Range(1, 10, ErrorMessage = "Ticket quantity must be between 1 and 10.")]
            public int Quantity { get; set; } = 1;
            // Used to receive dynamic fields
            public Dictionary<int, string> DynamicData { get; set; } = new Dictionary<int, string>();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MusicEvent = await _eventRepository.GetEventWithDetailsAsync(id.Value);

            if (MusicEvent == null)
            {
                return NotFound("Event not found.");
            }
            if (MusicEvent.RequiredFields != null)
            {
                foreach (var field in MusicEvent.RequiredFields)
                {
                    var defaultValue = (field.FieldType == "Checkbox") ? "false" : string.Empty;
                    Input.DynamicData.Add(field.FieldID, defaultValue);
                }
            }
            // Assign EventID to InputModel for posting
            Input.EventID = MusicEvent.EventID;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var musicEvent = await _eventRepository.GetEventWithDetailsAsync(Input.EventID);
            if (musicEvent == null)
            {
                return NotFound("Event no longer exists.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            // Check dynamic fields
            foreach (var field in musicEvent.RequiredFields.Where(f => f.IsRequired))
            {
                if (!Input.DynamicData.ContainsKey(field.FieldID) || string.IsNullOrWhiteSpace(Input.DynamicData[field.FieldID]))
                {
                    if (field.FieldType == "Checkbox")
                    {
                        ModelState.AddModelError($"Input.DynamicData[{field.FieldID}]", $"You must agree to '{field.FieldName}'.");
                    }
                    else
                    {
                        ModelState.AddModelError($"Input.DynamicData[{field.FieldID}]", $"Please enter information for '{field.FieldName}'.");
                    }
                }
                else if (field.FieldType == "Checkbox" && Input.DynamicData[field.FieldID] != "true")
                {
                    ModelState.AddModelError($"Input.DynamicData[{field.FieldID}]", $"You must agree to '{field.FieldName}'.");
                }
            }


            if (!ModelState.IsValid)
            {
                // If there are errors, reload the page with event info
                MusicEvent = musicEvent;
                return Page();
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            var result = await strategy.ExecuteAsync(async () =>
            {
                // Begin Transaction
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var selectedTier = await _context.PricingTiers
                        .FromSql($"SELECT * FROM PricingTiers WHERE PricingTierID = {Input.PricingTierID} FOR UPDATE")
                        .FirstOrDefaultAsync();

                    if (selectedTier == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid ticket type.");
                        return (IActionResult)Page();
                    }

                    // Check remaining ticket quantity (Concurrency Check)
                    if (selectedTier.AvailableTickets.HasValue)
                    {
                        int remaining = selectedTier.AvailableTickets.Value - selectedTier.SoldTickets;
                        if (remaining < Input.Quantity)
                        {
                            ModelState.AddModelError("Input.Quantity", $"Sorry, only {remaining} tickets are left for this tier.");
                            return (IActionResult)Page();
                        }
                    }

                    // Update sold tickets count
                    selectedTier.SoldTickets += Input.Quantity;

                    // Calculate total price
                    decimal totalPrice = selectedTier.Price * Input.Quantity;

                    // Create EventRegistration object
                    var registration = new EventRegistration
                    {
                        EventID = Input.EventID,
                        UserID = user.Id,
                        PricingTierID = selectedTier.PricingTierID,
                        TotalPrice = totalPrice,
                        RegistrationDate = DateTime.UtcNow,
                        Status = "Confirmed",
                        PaymentStatus = "Unpaid",
                        ConfirmationCode = Guid.NewGuid().ToString()
                    };

                    // Add dynamic data
                    if (Input.DynamicData != null)
                    {
                        foreach (var data in Input.DynamicData.Where(d => !string.IsNullOrWhiteSpace(d.Value)))
                        {
                            registration.RegistrationData.Add(new RegistrationData
                            {
                                FieldID = data.Key,
                                FieldValue = data.Value
                            });
                        }
                    }

                    // Save Registration (and RegistrationData, and update PricingTier)
                    _context.EventRegistrations.Add(registration);

                    // Save all changes to DB
                    await _context.SaveChangesAsync();

                    // Commit Transaction
                    await transaction.CommitAsync();

                    _logger.LogInformation("User {UserId} successfully registered for event {EventId}", user.Id, musicEvent.EventID);

                    // Redirect to success page
                    return (IActionResult)RedirectToPage("/Events/RegistrationSuccess", new { id = registration.RegistrationID });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Critical error when user {UserId} registered for event {EventId}", user.Id, Input.EventID);
                    ModelState.AddModelError(string.Empty, "A system error occurred. Please try again later.");
                    return (IActionResult)Page();
                }
            });

            // Return the result from within the strategy
            return result!;
        }
    }
}