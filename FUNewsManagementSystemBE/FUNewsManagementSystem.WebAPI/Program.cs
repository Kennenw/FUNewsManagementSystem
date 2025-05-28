
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.Repository;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;

namespace FUNewsManagementSystem.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
            builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();

            var odataBuilder = new ODataConventionModelBuilder();
                odataBuilder.EntitySet<SystemAccount>("SystemAccounts");
                odataBuilder.EntitySet<NewsArticle>("NewsArticles");

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                })
                .AddOData(
                    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
                        "odata",
                        odataBuilder.GetEdmModel()));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigins",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7149")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseODataBatching();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseCors("AllowOrigins");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
