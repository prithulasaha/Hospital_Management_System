using HMSphere.Application.Interfaces;
using HMSphere.Application.Services;
using HMSphere.Domain.Entities;
using HMSphere.Infrastructure.DataContext;
using HMSphere.MVC.AutoMapper;
using HMSphere.MVC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HMSphere.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using HMSphere.Application.Mailing;
namespace HMSphere.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password Settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout Settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User Settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });


            //Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<HmsContext>().AddDefaultTokenProviders();

            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy",
                    corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod());
            });

            //Add DbContext
            builder.Services.AddDbContext<HmsContext>(options =>
            options.UseSqlServer(builder.Configuration
            .GetConnectionString("DefaultConnection")));

			//email
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mailing"));

            //configure  Services
            builder.Services.AddScoped(typeof(IAccountService), typeof(AccountService));
            builder.Services.AddScoped<IMailingService, MailingService>();
            builder.Services.AddScoped<IUserRoleFactory, UserRoleFactory>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
			builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IShiftService, ShiftService>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IStaffService, StaffService>();


            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mailing"));


            //seeding Data
            builder.Services.AddScoped<StoredContextSeed>();
            // builder.Services.AddScoped<IdentitySeed>();

            #region jwt
            //builder.Services.AddAuthentication(options =>
            //{
            //	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //	options.RequireHttpsMetadata = false;
            //	options.SaveToken = true;
            //	options.TokenValidationParameters = new TokenValidationParameters
            //	{
            //		ValidateIssuerSigningKey = true,
            //		ValidateLifetime = true,
            //		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            //		ValidateIssuer = true,
            //		ValidIssuer = builder.Configuration["JWT:issuer"],
            //		ValidateAudience = true,
            //		ValidAudience = builder.Configuration["JWT:audience"],
            //		ClockSkew = TimeSpan.Zero // Optional: reduce the default clock skew
            //	};
            //});
            #endregion

			#region cookies
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
				.AddCookie(options =>
				{
					options.LoginPath = "/Account/Login";
				});
			#endregion

			builder.Services.AddControllersWithViews();
            builder.Services.AddAuthorization();

            var app = builder.Build();
			//For Seeding Data 
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<HmsContext>();
				var usermanager = services.GetRequiredService<UserManager<ApplicationUser>>();
				//await StoredContextSeed.SeedUserAsync(usermanager,context);
				// await IdentitySeed.SeedUserAsync(usermanager);
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<PerformanceMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Register}/{id?}");

            app.Run();
        }
    }
}
