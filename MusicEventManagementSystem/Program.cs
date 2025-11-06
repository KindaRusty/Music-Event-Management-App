using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services; // Đảm bảo using này
using MusicEventManagementSystem.Services.Implementations;
using MusicEventManagementSystem.Services.Interfaces;
// using MusicEventManagementSystem.Services.Interfaces; // Dòng này có thể không cần nếu bạn không tạo thư mục Interfaces

var builder = WebApplication.CreateBuilder(args);

// 1. Lấy chuỗi kết nối (Connection String)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký DbContext với MySQL
builder.Services.AddDbContext<MusicDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

// 3. Đăng ký Identity (Sử dụng ApplicationUser)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MusicDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// 4. Đăng ký các dịch vụ (Dependency Injection)
builder.Services.AddScoped<IMusicEventRepository, MusicEventRepository>();
builder.Services.AddScoped<IAiService, AiApiService>(); // Giữ lại 1 dòng này

// 5. XÓA BỎ CÁC DÒNG LỖI
// builder.Services.AddScoped<IAiService, AiApiService>(); // XÓA DÒNG BỊ LẶP
// builder.Services.Configure<AiSettings>(builder.Configuration.GetSection("AiSettings")); // XÓA DÒNG GÂY LỖI "AiSettings"

// 7. Thêm dịch vụ Razor Pages
builder.Services.AddRazorPages();

// 8. Định nghĩa Policy "Admin"
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});

// 9. Add Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Đảm bảo 2 dòng này đúng thứ tự
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();