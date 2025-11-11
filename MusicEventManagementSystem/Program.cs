using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using MusicEventManagementSystem.Services.Implementations;
using MusicEventManagementSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register DbContext with MySQL
builder.Services.AddDbContext<MusicDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

// 3. Register Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MusicDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// 4. Register application services
builder.Services.AddScoped<IMusicEventRepository, MusicEventRepository>();
builder.Services.AddScoped<IAiService, AiApiService>();

// 5. Add razor pages with runtime compilation
builder.Services.AddRazorPages();

// 6. Define admin policy for authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});

// 7. Register email service
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();