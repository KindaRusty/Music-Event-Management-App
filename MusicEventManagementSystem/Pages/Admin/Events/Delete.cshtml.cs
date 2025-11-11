using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces; // Fix namespace
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IMusicEventRepository _repository;

        public DeleteModel(IMusicEventRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public MusicEvent MusicEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fix: Use GetEventByIdAsync
            MusicEvent = await _repository.GetEventByIdAsync(id.Value);

            if (MusicEvent == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fix: Use DeleteEventAsync
            await _repository.DeleteEventAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}