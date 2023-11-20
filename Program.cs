using Microsoft.AspNetCore.Identity.UI.Services;
using linc.Models.ConfigModels;
using linc.Services;
using linc.Utility;

namespace linc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var services = builder.Services;

            //services.AddOptions();

            // Options -->
            services.Configure<EmailConfig>(configuration.GetSection(nameof(EmailConfig)));

            // Services -->
            services.AddTransient<IEmailSender, EmailSender>();

            // Database -->
            services.AddDatabase(configuration);

            // Add services to the container.
            services.AddControllersWithViews();

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}