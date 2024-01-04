using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace linc.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public readonly DbContextOptions<ApplicationDbContext> Options;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Options = options;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasData(new ApplicationUser
            {
                Id = SiteConstant.ZeroGuid,
                ConcurrencyStamp = SiteConstant.ZeroGuid,
                SecurityStamp = SiteConstant.ZeroGuid,

                PasswordHash = "CHANGE_ME",

                UserName = SiteConstant.AdministratorUserName,
                NormalizedUserName = SiteConstant.AdministratorUserName.ToUpper(),
                FirstName = SiteConstant.AdministratorFirstName,
                LastName = SiteConstant.AdministratorLastName,
                Email = SiteConstant.AdministratorEmail,
                NormalizedEmail = SiteConstant.AdministratorEmail.ToUpper(),
                Description = "System administrator. / Администратор на системата.",

                AvatarType = UserAvatarType.Gravatar,
                DisplayNameType = UserDisplayNameType.NamesAndUserName,
                DisplayEmail = true,
                EmailConfirmed = true,

                DateCreated = DateTime.Parse("2024-01-01"),
                LastUpdated = DateTime.Parse("2024-01-01"),
            });

        foreach (var (id, name) in SiteRolesHelper.DatabaseRolesSeed)
        {
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = id.ToString(),
                    ConcurrencyStamp = new string(id.ToString().Reverse().ToArray()),
                    NormalizedName = name,
                    Name = name
                });
        }

        builder.Entity<IdentityUserRole<string>>()
            .HasData(new IdentityUserRole<string>()
            {
                UserId = SiteConstant.ZeroGuid,
                RoleId = SiteConstant.ZeroGuid
            });
    }
}
