using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces; // Fix namespace
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Events
{
    public class DetailModel : PageModel
    {
        private readonly IMusicEventRepository _repository;

        public DetailModel(IMusicEventRepository repository)
        {
            _repository = repository;
        }

        public MusicEvent MusicEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Edit: Use GetEventByIdAsync
            MusicEvent = await _repository.GetEventByIdAsync(id.Value);

            if (MusicEvent == null)
            {
                return NotFound("Event not found.");
            }
            return Page();
        }
    }
}