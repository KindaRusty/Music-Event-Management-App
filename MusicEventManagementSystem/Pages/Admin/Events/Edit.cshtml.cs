using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IMusicEventRepository repository,
            UserManager<ApplicationUser> userManager,
            ILogger<EditModel> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        [BindProperty]
        public List<PricingTierInput> PricingTiers { get; set; } = new List<PricingTierInput>();

        [BindProperty]
        public List<RequiredFieldInput> RequiredFields { get; set; } = new List<RequiredFieldInput>();

        #region Input DTOs
        public class InputModel
        {
            [Required]
            public int EventID { get; set; }

            [Required(ErrorMessage = "Event name is required")]
            [StringLength(200, MinimumLength = 5)]
            public string EventName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Location is required")]
            public string Location { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event date is required")]
            public DateTime EventDate { get; set; }

            public string? Genre { get; set; }
            public int? MaxAttendees { get; set; }
            public string? Description { get; set; }
            public bool IsPublished { get; set; }
            public string? ImageUrl { get; set; }

            public string CreatedByUserID { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }
        public class PricingTierInput
        {
            public int PricingTierID { get; set; }

            [Required]
            public string TierName { get; set; } = string.Empty;
            [Required]
            [Range(0, double.MaxValue)]
            public decimal Price { get; set; }
            public int? AvailableTickets { get; set; }
            public string? Description { get; set; }
        }
        public class RequiredFieldInput
        {
            public int FieldID { get; set; }

            [Required]
            public string FieldName { get; set; } = string.Empty;
            [Required]
            public string FieldType { get; set; } = "Text";
            public bool IsRequired { get; set; } = true;
            public int DisplayOrder { get; set; }
        }
        #endregion

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicEvent = await _repository.GetEventWithDetailsAsync(id.Value);

            if (musicEvent == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                EventID = musicEvent.EventID,
                EventName = musicEvent.EventName,
                Location = musicEvent.Location,
                EventDate = musicEvent.EventDate,
                Genre = musicEvent.Genre,
                MaxAttendees = musicEvent.MaxAttendees,
                Description = musicEvent.Description,
                IsPublished = musicEvent.IsPublished,
                CreatedByUserID = musicEvent.CreatedByUserID,
                CreatedDate = musicEvent.CreatedDate,
                ImageUrl = musicEvent.ImageUrl
            };

            PricingTiers = musicEvent.PricingTiers.Select(p => new PricingTierInput
            {
                PricingTierID = p.PricingTierID,
                TierName = p.TierName,
                Price = p.Price,
                AvailableTickets = p.AvailableTickets,
                Description = p.Description
            }).ToList();

            RequiredFields = musicEvent.RequiredFields.Select(f => new RequiredFieldInput
            {
                FieldID = f.FieldID,
                FieldName = f.FieldName,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                DisplayOrder = f.DisplayOrder
            }).OrderBy(f => f.DisplayOrder).ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var eventToUpdate = new MusicEvent
            {
                EventID = Input.EventID,
                EventName = Input.EventName,
                Location = Input.Location,
                EventDate = Input.EventDate,
                Genre = Input.Genre,
                MaxAttendees = Input.MaxAttendees,
                Description = Input.Description,
                IsPublished = Input.IsPublished,
                ImageUrl = Input.ImageUrl,

                CreatedByUserID = Input.CreatedByUserID,
                CreatedDate = Input.CreatedDate,
                LastModifiedDate = DateTime.UtcNow,
                PricingTiers = PricingTiers.Select(p => new PricingTier
                {
                    PricingTierID = p.PricingTierID,
                    TierName = p.TierName,
                    Price = p.Price,
                    AvailableTickets = p.AvailableTickets,
                    Description = p.Description,
                    EventID = Input.EventID
                }).ToList(),
                RequiredFields = RequiredFields.Select(f => new RequiredField
                {
                    FieldID = f.FieldID,
                    FieldName = f.FieldName,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    DisplayOrder = f.DisplayOrder,
                    EventID = Input.EventID
                }).ToList()
            };

            try
            {
                await _repository.UpdateEventAsync(eventToUpdate);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error while updating event {EventId}", Input.EventID);
                ModelState.AddModelError(string.Empty, "The data was modified by another user. Please reload the page.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", Input.EventID);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the event.");
                return Page();
            }

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}