using Microsoft.AspNetCore.Authentication.Cookies;

namespace FUNewsManagementSystem.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.LoginPath = "/Auth/Index"; // Chuyển hướng đến trang đăng nhập nếu chưa xác thực
                                options.AccessDeniedPath = "/Auth/AccessDenied"; // Trang khi truy cập không có quyền
                                options.ExpireTimeSpan = TimeSpan.FromDays(1); // Thời gian hết hạn của cookie xác thực
                                options.SlidingExpiration = true; // Gia hạn cookie khi người dùng hoạt động
                            });
            // Add services to the container.
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Staff", policy =>
                    policy.RequireClaim("Role", "1"));

                options.AddPolicy("Lecturer", policy =>
                    policy.RequireClaim("Role", "2"));

                options.AddPolicy("Admin", policy =>
                    policy.RequireClaim("Role", "3"));
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
