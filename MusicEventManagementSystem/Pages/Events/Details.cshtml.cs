using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
// Sửa: Đảm bảo using đúng cho repository của bạn
using MusicEventManagementSystem.Services;
using MusicEventManagementSystem.Services.Interfaces;
using System; // Thêm
using System.Security.Claims; // Thêm
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Events
{
    // Tên Model của bạn là DetailModel (không phải DetailsModel)
    public class DetailModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        // Các thuộc tính để kiểm soát UI
        public bool IsEventOver { get; set; } = false;
        public bool IsRegistrationClosed { get; set; } = false;
        public bool IsAlreadyRegistered { get; set; } = false;
        public int UserRegistrationId { get; set; }

        // SỬA LỖI: Thêm UserManager<ApplicationUser> vào constructor
        public DetailModel(IMusicEventRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager; // Dòng này giờ sẽ hoạt động
        }

        public MusicEvent MusicEvent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Giả sử GetEventByIdAsync tải đủ thông tin (PricingTiers, RequiredFields)
            MusicEvent = await _repository.GetEventByIdAsync(id.Value);

            if (MusicEvent == null)
            {
                return NotFound("Event not found.");
            }

            // SỬA LỖI: Toàn bộ logic này đã bị bỏ qua
            var now = DateTime.UtcNow;

            // 1. Kiểm tra nếu sự kiện đã kết thúc
            if (MusicEvent.EndDate.HasValue && MusicEvent.EndDate < now)
            {
                IsEventOver = true;
            }
            // 2. Kiểm tra nếu sự kiện (không có EndDate) đã qua (giả sử 1 ngày)
            else if (!MusicEvent.EndDate.HasValue && MusicEvent.EventDate < now.AddDays(-1))
            {
                IsEventOver = true;
            }

            // 3. Kiểm tra deadline đăng ký
            if (MusicEvent.RegistrationDeadline.HasValue && MusicEvent.RegistrationDeadline < now)
            {
                IsRegistrationClosed = true;
            }

            // 4. Kiểm tra xem người dùng đã đăng ký chưa
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                // Bạn cần một phương thức trong repository để làm việc này
                // (Giả sử bạn tự thêm phương thức này vào Repository)
                /* var registration = await _repository.GetUserRegistrationAsync(id.Value, userId);
                if (registration != null)
                {
                    IsAlreadyRegistered = true;
                    UserRegistrationId = registration.RegistrationID;
                }
                */
                // Nếu bạn chưa có repository, tạm thời dùng _context (nếu bạn có)
                // Vì bạn dùng Repository, tôi sẽ bỏ qua phần này, giả sử Model.IsAlreadyRegistered bạn tự xử lý
            }
            return Page();
        }
    }
}