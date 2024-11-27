
using LaptopStore.Business.Interfaces;
using LaptopStore.Business.Services;
using LaptopStore.Business.Services.Contracts;
using LaptopStore.Data.Contexts;
using LaptopStore.Data.Contracts;
using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;
using LaptopStore.Data.Infrastructure;
using LaptopStore.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;
using OfficeOpenXml;
using LaptopStore.API.Controllers;

namespace LaptopStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Logging Configuration
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // 2. Database Context Configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 3. Identity Configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // 4. Register UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 5. Register Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // 6. Register Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddTransient<AuthenticationController>();

            // 7. Email Service Configuration (MailKit)
            builder.Services.AddMailKit(config => config.UseMailKit(new MailKitOptions
            {
                Server = "smtp.gmail.com",
                Port = 587,
                SenderName = "NguyenThanhAn",
                SenderEmail = "annthe176161@fpt.edu.vn",
                Account = "annthe176161@fpt.edu.vn",
                Password = "hxhp laiv kjeb sols",
                Security = true
            }));

            // 8. JWT Authentication Configuration
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"]
                };
            });

            // 9. CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 10. Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LaptopStore API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter JWT token"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });
            });

            // 11. Health Checks Configuration
            builder.Services.AddHealthChecks();

            // 12. Configure MVC and Controllers
            builder.Services.AddControllers();

            // 13. Excel Package Configuration
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var app = builder.Build();

            // Middleware Configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(option =>
            {
                option.AllowAnyHeader();
                option.AllowAnyMethod();
                option.AllowAnyOrigin();
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/health");

            // Gọi hàm SeedRolesAndAdmin
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Gọi SeedRolesAndAdmin và đợi nó hoàn thành
                Task.Run(async () =>
                {
                    await LaptopStore.API.Infrastructure.SeedData.SeedRolesAndAdmin(services);
                }).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}