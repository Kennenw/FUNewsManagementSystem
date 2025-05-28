
using FUNewsManagementSystem.Reposirories;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.Repository;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace FUNewsManagementSystem.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            var AdminRoleId = configuration["Roles:AdminRoleId"];

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
            builder.Services.AddScoped<ICategoryService, CategoryService>();


            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<SystemAccount>("SystemAccounts");
            odataBuilder.EntitySet<NewsArticle>("NewsArticles");
            odataBuilder.EntitySet<Category>("Categories");
            odataBuilder.EntitySet<Tag>("Tags");

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

            builder.Services.AddAuthentication(x =>
                            {
                                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(x =>
                            {
                                x.SaveToken = true;
                                x.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = false,
                                    ValidateAudience = false,
                                    ValidateLifetime = false,
                                    ValidIssuer = configuration["JWT:Issuer"],
                                    ValidAudience = configuration["JWT:Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
                                };
                            });

            // Add Swagger JWT configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "JWT Authentication for Cosmetics Management",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                };

                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                                            {
                                                {
                                                    new OpenApiSecurityScheme
                                                    {
                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.SecurityScheme,
                                                            Id = "Bearer"
                                                        }
                                                    },
                                                    new string[] {}
                                                }
                                            };
                c.ResolveConflictingActions(apiDescriptions =>
                {
                    return apiDescriptions.First();
                });

                c.AddSecurityRequirement(securityRequirement);
            });

            builder.Services.AddAuthorization(options =>
            {

                options.AddPolicy("Staff",
                    policyBuilder => policyBuilder.RequireAssertion(
                        context => context.User.HasClaim(claim => claim.Type == "Role") &&
                        context.User.FindFirst(claim => claim.Type == "Role").Value == "1"));

                options.AddPolicy("Lecturer",
                    policyBuilder => policyBuilder.RequireAssertion(
                        context => context.User.HasClaim(claim => claim.Type == "Role")
                        && (context.User.FindFirst(claim => claim.Type == "Role").Value == "2")));

                options.AddPolicy("Admin",
                    policyBuilder => policyBuilder.RequireAssertion(
                        context => context.User.HasClaim(claim => claim.Type == "Role")
                        && (context.User.FindFirst(claim => claim.Type == "Role").Value == AdminRoleId)));
                }
            );

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

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

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
