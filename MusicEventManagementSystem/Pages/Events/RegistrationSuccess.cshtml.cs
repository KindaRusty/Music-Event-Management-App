using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Cần cho Include
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegistrationSuccessModel : PageModel
    {
        private readonly MusicDbContext _context;

        public RegistrationSuccessModel(MusicDbContext context)
        {
            _context = context;
        }

        // Dùng để hiển thị thông tin
        public EventRegistration Registration { get; set; } = default!;
        public List<RegistrationData> DynamicData { get; set; } = new List<RegistrationData>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound("Không tìm thấy mã đăng ký.");
            }

            // Tải thông tin đăng ký KÈM THEO các thông tin liên quan
            Registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)        // Lấy tên sự kiện
                .Include(r => r.ApplicationUser)  // Lấy email user
                .Include(r => r.PricingTier)      // Lấy tên loại vé
                .FirstOrDefaultAsync(r => r.RegistrationID == id.Value);

            if (Registration == null)
            {
                // Có thể user đang xem đăng ký của người khác
                return Forbid("Bạn không có quyền xem đăng ký này hoặc đăng ký không tồn tại.");
            }

            // Tải riêng các dữ liệu động (dynamic fields)
            // (Bao gồm tên của trường)
            DynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField) // Lấy tên của trường (Label)
                .Where(d => d.RegistrationID == id.Value)
                .OrderBy(d => d.RequiredField.DisplayOrder) // Sắp xếp
                .ToListAsync();

            return Page();
        }
    }
}