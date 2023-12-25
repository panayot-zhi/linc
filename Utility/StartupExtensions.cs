using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using linc.Data;
using Microsoft.AspNetCore.Mvc;
using linc.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using linc.Models.ConfigModels;

namespace linc.Utility;

public static class StartupExtensions
{
    public static IServiceCollection AddAuthentications(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
            // .AddFacebook(options =>
            // {
            //     options.AppId = configuration.GetValue<string>("FacebookAppId");
            //     options.AppSecret = configuration.GetValue<string>("FacebookAppSecret");
            //     options.Fields.Add("picture.width(150).height(150)");
            //     options.ClaimActions.MapCustomJson(StranitzaClaimTypes.Picture,
            //         json => json.GetProperty("picture").GetProperty("data").GetString("url"));
            //     //options.Events.OnCreatingTicket = OnCreatingTicket;
            // })
            // .AddTwitter(options =>
            // {
            //     options.ConsumerKey = configuration.GetValue<string>("TwitterConsumerKey");
            //     options.ConsumerSecret = configuration.GetValue<string>("TwitterConsumerSecret");
            //     options.ClaimActions.MapJsonKey(StranitzaClaimTypes.Picture, "profile_image_url_https");
            //     //options.Events.OnCreatingTicket = OnCreatingTicket;
            //     options.RetrieveUserDetails = true;
            // })
            // .AddGoogle(options =>
            // {
            //     options.ClientId = configuration.GetValue<string>("GoogleClientId");
            //     options.ClientSecret = configuration.GetValue<string>("GoogleClientSecret");
            //     options.ClaimActions.MapJsonKey(StranitzaClaimTypes.Picture, "picture");
            //     options.ClaimActions.MapJsonKey(StranitzaClaimTypes.VerifiedEmail, "verified_email");
            //     //options.Events.OnCreatingTicket = OnCreatingTicket;
            // });

        return services;
    }

    public static IServiceCollection AddCachingProfiles(this IServiceCollection services)
    {
        services.AddResponseCaching();

        services.AddControllersWithViews(options =>
        {
            options.CacheProfiles.Add(SiteCacheProfile.NoCache, new CacheProfile()
            {
                Duration = 0,
                Location = ResponseCacheLocation.None,
                NoStore = true
            });

            options.CacheProfiles.Add(SiteCacheProfile.Hourly, new CacheProfile()
            {
                Duration = 60 * 60 // 1 hour
            });

            options.CacheProfiles.Add(SiteCacheProfile.Weekly, new CacheProfile()
            {
                Duration = 60 * 60 * 24 * 7 // 7 days
            });

            options.CacheProfiles.Add(SiteCacheProfile.Monthly, new CacheProfile()
            {
                Duration = 60 * 60 * 24 * 30 // 30 days
            });

            options.CacheProfiles.Add(SiteCacheProfile.Yearly, new CacheProfile()
            {
                Duration = 60 * 60 * 24 * 365 // 365 days
            });

        });

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailConfig>(configuration.GetSection(nameof(EmailConfig)));

        return services;
    }

    public static IServiceCollection AddCookies(this IServiceCollection services)
    {
        // .netCore.linc.identity.external
        services.ConfigureExternalCookie(options =>
        {
            options.Cookie.HttpOnly = true;

            // no cookie policy enforces this
            // if you logged in - you gave consent
            options.Cookie.IsEssential = true;
            options.Cookie.Name = ".netCore.linc.identity.external";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            // If the LoginPath is not set here,
            // ASP.NET Core will default to /Account/Login
            options.LoginPath = "/Identity/Account/Login";

            // If the LogoutPath is not set here,
            // ASP.NET Core will default to /Account/Logout
            options.LogoutPath = "/Identity/Account/Logout";

            // If the AccessDeniedPath is
            // not set here, ASP.NET Core 
            // will default to 
            // /Account/AccessDenied
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";

            // allow obtaining new ticket
            // on a new user activity
            // stick with the defaults
            // options.SlidingExpiration = true;
        });

        // .netCore.linc.identity
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;

            // no cookie policy enforces this
            // if you logged in - you gave consent
            options.Cookie.IsEssential = true;
            options.Cookie.Name = ".netCore.linc.identity";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            // This interferes with the remember me feature
            //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

            // If the LoginPath is not set here,
            // ASP.NET Core will default to /Account/Login
            options.LoginPath = "/Identity/Account/Login";

            // If the LogoutPath is not set here,
            // ASP.NET Core will default to /Account/Logout
            options.LogoutPath = "/Identity/Account/Logout";

            // If the AccessDeniedPath is
            // not set here, ASP.NET Core 
            // will default to 
            // /Account/AccessDenied
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";

            // allow obtaining new ticket
            // on a new user activity
            // stick with the defaults
            // options.SlidingExpiration = true;

        });

        // .netCore.linc.tempData
        services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = false;
            options.Cookie.Name = ".netCore.linc.tempData";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        // .netCore.linc.antiForgery
        services.AddAntiforgery(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = ".netCore.linc.antiForgery";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

            // stick with the defaults
            // options.Cookie.Expiration = TimeSpan.FromMinutes(30);
        });

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(
            contextLifetime: ServiceLifetime.Scoped,
            optionsAction: options =>
                options
                    .EnableDetailedErrors()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        services.Configure<IdentityOptions>(options =>
        {
            // Store settings
            options.Stores.MaxLengthForKeys = 127;

            // Password Strength settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 7;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = Constants.AllowedUserNameCharacters;

            // SignIn options
            options.SignIn.RequireConfirmedEmail = true;
        });

        services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
        });

        return services;
    }

    public static IServiceCollection AddRoutes(this IServiceCollection services)
    {
        services.AddRouting(option =>
        {
            option.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            option.LowercaseUrls = true;
        });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }

    public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T : DbContext
    {
        var context = app.ApplicationServices.GetRequiredService<T>();
        context.Database.Migrate();
    }
}