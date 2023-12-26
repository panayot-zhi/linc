using linc.Utility;
using Microsoft.AspNetCore.Mvc.Razor;

namespace linc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        var services = builder.Services;
        var environment = builder.Environment;
        var host = builder.Host;

        services
            .AddDatabase(configuration)
            .AddConfigurations(configuration)
            .AddAuthentications(configuration)
            .AddApplicationIdentity()
            .AddCachingProfiles()
            .AddLocalizations()
            .AddCookies()
            .AddServices()
            .AddRoutes();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!environment.IsDevelopment())
        {
            // The default HSTS value is 30 days.
            // You may want to change this for production scenarios,
            // see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseResponseCaching();
        app.UseRequestLocalization();
        app.UseExceptionHandler("/Home/Error");
        app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        //app.EnsureMigrationOfContext<ApplicationDbContext>();

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