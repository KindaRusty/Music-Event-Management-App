using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;

namespace MusicEventManagementSystem.Pages.Events
{
    public class DetailModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        public bool IsEventOver { get; set; } = false;
        public bool IsRegistrationClosed { get; set; } = false;
        public bool IsAlreadyRegistered { get; set; } = false;
        public int UserRegistrationId { get; set; }
        public DetailModel(IMusicEventRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public MusicEvent MusicEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MusicEvent = await _repository.GetEventByIdAsync(id.Value);

            if (MusicEvent == null)
            {
                return NotFound("Event not found.");
            }
            var now = DateTime.UtcNow;
            if (MusicEvent.EndDate.HasValue && MusicEvent.EndDate < now)
            {
                IsEventOver = true;
            }
            // 2. Check if event without EndDate is over
            else if (!MusicEvent.EndDate.HasValue && MusicEvent.EventDate < now.AddDays(-1))
            {
                IsEventOver = true;
            }

            // 3. Check registration deadline
            if (MusicEvent.RegistrationDeadline.HasValue && MusicEvent.RegistrationDeadline < now)
            {
                IsRegistrationClosed = true;
            }

            // 4. Check if the user is already registered
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
            }
            return Page();
        }
    }
}