using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using MusicEventManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicEventManagementSystem.Data; // THÊM
using Microsoft.EntityFrameworkCore; // THÊM
using System.Security.Claims; // THÊM
using System; // THÊM

namespace MusicEventManagementSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly ILogger<IndexModel> _logger;
        private readonly IAiService _aiService;
        private readonly MusicDbContext _context; // THÊM

        // SỬA CONSTRUCTOR: Thêm MusicDbContext
        public IndexModel(IMusicEventRepository repository, ILogger<IndexModel> logger, IAiService aiService, MusicDbContext context)
        {
            _repository = repository;
            _logger = logger;
            _aiService = aiService;
            _context = context; // THÊM
        }

        // SỬA TÊN: Giữ nguyên tên biến UpcomingEvents
        public IEnumerable<MusicEvent> UpcomingEvents { get; set; } = Enumerable.Empty<MusicEvent>();

        // THÊM CÁC THUỘC TÍNH NÀY
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GenreFilter { get; set; }

        public List<string> Genres { get; set; } = new List<string>();
        public List<MusicEvent> RecommendedEvents { get; set; } = new List<MusicEvent>();
        // KẾT THÚC THÊM

        public async Task<IActionResult> OnPostChatAsync([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return new JsonResult(new { reply = "Vui lòng nhập tin nhắn." });
            }

            var aiReply = await _aiService.GetChatReplyAsync(request.Message);
            return new JsonResult(new { reply = aiReply });
        }

        // SỬA LẠI: OnGetAsync
        public async Task OnGetAsync()
        {
            try
            {
                // Lấy danh sách thể loại để điền vào dropdown
                Genres = await _context.MusicEvents
                    .Where(e => e.Genre != null && e.Genre != "")
                    .Select(e => e.Genre)
                    .Distinct()
                    .OrderBy(g => g)
                    .ToListAsync();

                // SỬA: Lấy sự kiện đã lọc thay vì tất cả
                UpcomingEvents = await _repository.GetFilteredEventsAsync(SearchTerm, GenreFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách sự kiện trên trang chủ.");
            }
        }

        // THÊM HANDLER NÀY
        public async Task<IActionResult> OnGetRecommendationsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                RecommendedEvents = await _aiService.GetEventRecommendationsAsync(userId);
            }

            // Tải lại dữ liệu (bao gồm cả Genres và UpcomingEvents đã lọc)
            await OnGetAsync();
            return Page();
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}