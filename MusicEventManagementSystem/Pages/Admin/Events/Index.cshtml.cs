using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IMusicEventRepository _repository;

        public IndexModel(IMusicEventRepository repository)
        {
            _repository = repository;
        }
        public IList<MusicEvent> MusicEvents { get; set; } = new List<MusicEvent>();

        public async Task OnGetAsync()
        {
            var events = await _repository.GetAllEventsForAdminAsync();

            MusicEvents = events.OrderByDescending(e => e.CreatedDate).ToList();
        }
    }
}