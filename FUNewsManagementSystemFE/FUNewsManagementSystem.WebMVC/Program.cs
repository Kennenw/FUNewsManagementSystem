using Microsoft.AspNetCore.Authentication.Cookies;

namespace FUNewsManagementSystem.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình xác thực bằng Cookie
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.LoginPath = "/Auth/Index";
                                options.AccessDeniedPath = "/Auth/AccessDenied";
                                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                                options.SlidingExpiration = true;
                            });

            // Cấu hình phân quyền
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Staff", policy => policy.RequireClaim("Role", "1"));
                options.AddPolicy("Lecturer", policy => policy.RequireClaim("Role", "2"));
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "3"));
            });

            // Thêm Session support
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 👉 Thêm Session middleware vào đúng vị trí
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
