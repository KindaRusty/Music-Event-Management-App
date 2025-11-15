using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using MusicEventManagementSystem.Services.Interfaces;
using MusicEventManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace MusicEventManagementSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly ILogger<IndexModel> _logger;
        private readonly IAiService _aiService;
        private readonly MusicDbContext _context;
        public IndexModel(IMusicEventRepository repository, ILogger<IndexModel> logger, IAiService aiService, MusicDbContext context)
        {
            _repository = repository;
            _logger = logger;
            _aiService = aiService;
            _context = context;
        }
        public IEnumerable<MusicEvent> UpcomingEvents { get; set; } = Enumerable.Empty<MusicEvent>();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GenreFilter { get; set; }

        public List<string> Genres { get; set; } = new List<string>();
        public List<MusicEvent> RecommendedEvents { get; set; } = new List<MusicEvent>();
        public async Task<IActionResult> OnPostChatAsync([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return new JsonResult(new { reply = "Please enter a message." });
            }

            var aiReply = await _aiService.GetChatReplyAsync(request.Message);
            return new JsonResult(new { reply = aiReply });
        }
        public async Task OnGetAsync()
        {
            try
            {
                // Get the list of genres to populate the dropdown
                Genres = await _context.MusicEvents
                    .Where(e => e.Genre != null && e.Genre != "")
                    .Select(e => e.Genre)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToListAsync();
                UpcomingEvents = await _repository.GetFilteredEventsAsync(SearchTerm, GenreFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event list on the homepage.");
            }
        }
        public async Task<IActionResult> OnGetRecommendationsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                RecommendedEvents = await _aiService.GetEventRecommendationsAsync(userId);
            }

            await OnGetAsync();
            return Page();
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}