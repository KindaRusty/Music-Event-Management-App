using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces; // Đảm bảo có using
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Thêm using này

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

        // Khởi tạo List để tránh lỗi null
        public IList<MusicEvent> MusicEvents { get; set; } = new List<MusicEvent>();

        public async Task OnGetAsync()
        {
            // [SỬA LỖI Ở ĐÂY]
            // Chúng ta gọi tên phương thức mới đã tạo: GetAllEventsForAdminAsync()
            // Phương thức này trả về IEnumerable, nên chúng ta .ToList() nó.
            var events = await _repository.GetAllEventsForAdminAsync();

            // Sắp xếp sau khi đã lấy dữ liệu
            MusicEvents = events.OrderByDescending(e => e.CreatedDate).ToList();
        }
    }
}