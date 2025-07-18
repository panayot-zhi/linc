using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace linc.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public const string AutoUpdateProperty = "LastUpdated";
    public const string AutoCreateProperty = "DateCreated";

    public readonly DbContextOptions<ApplicationDbContext> Options;

    
    public DbSet<ApplicationAuthor> Authors { get; set; }

    public DbSet<ApplicationIssue> Issues { get; set; }

    public DbSet<IssueJournal> IssueJournals { get; set; }


    public DbSet<ApplicationSource> Sources { get; set; }

    public DbSet<SourceJournal> SourceJournals { get; set; }


    public DbSet<ApplicationDossier> Dossiers { get; set; }

    public DbSet<DossierJournal> DossierJournals { get; set; }


    public DbSet<ApplicationDossierReview> DossierReviews { get; set; }

    public DbSet<ApplicationDocument> Documents { get; set; }

    public DbSet<ApplicationLanguage> Languages { get; set; }

    public DbSet<ApplicationStringResource> StringResources { get; set; }

    public DbSet<ApplicationUserProfile> UserProfiles { get; set; }
    

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Options = options;
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        TrackCreatedEntities();
        TrackUpdatedEntities();
        StandardizeAuthorNames();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected void TrackUpdatedEntities()
    {
        var editedEntities = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Modified).ToList();

        editedEntities.ForEach(entry =>
        {
            if (entry.HasProperty(AutoUpdateProperty))
            {
                entry.Property(AutoUpdateProperty).CurrentValue = DateTime.UtcNow;
            }

            if (entry.HasProperty(AutoCreateProperty))
            {
                entry.Property(AutoCreateProperty).IsModified = false;
            }
        });
    }

    protected void TrackCreatedEntities()
    {
        var addedEntities = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Added).ToList();

        var now = DateTime.UtcNow;
        addedEntities.ForEach(entry =>
        {
            if (entry.HasProperty(AutoCreateProperty))
            {
                entry.Property(AutoCreateProperty).CurrentValue = now;
            }

            if (entry.HasProperty(AutoUpdateProperty))
            {
                entry.Property(AutoUpdateProperty).CurrentValue = now;
            }
        });
    }

    private void StandardizeAuthorNames()
    {
        var authorEntries = ChangeTracker.Entries<ApplicationAuthor>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in authorEntries)
        {
            var author = entry.Entity;
            if (!string.IsNullOrWhiteSpace(author.FirstName))
            {
                author.FirstName = char.ToUpper(author.FirstName[0]) + author.FirstName[1..].ToLower();
            }

            if (!string.IsNullOrWhiteSpace(author.LastName))
            {
                author.LastName = author.LastName.ToUpper();
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Snake case naming convention for identity tables
        builder.Entity<IdentityRole>(x => x.ToTable("asp_net_roles"));
        builder.Entity<ApplicationUser>(x => x.ToTable("asp_net_users"));
        builder.Entity<ApplicationUserProfile>(x => x.ToTable("asp_net_user_profiles"));
        builder.Entity<IdentityRoleClaim<string>>(x => x.ToTable("asp_net_role_claims"));
        builder.Entity<IdentityUserClaim<string>>(x => x.ToTable("asp_net_user_claims"));
        builder.Entity<IdentityUserLogin<string>>(x => x.ToTable("asp_net_user_logins"));
        builder.Entity<IdentityUserRole<string>>(x => x.ToTable("asp_net_user_roles"));
        builder.Entity<IdentityUserToken<string>>(x => x.ToTable("asp_net_user_tokens"));

        // set primary composite keys
        builder.Entity<ApplicationUserProfile>()
            .HasKey(x => new { x.UserId, x.LanguageId });

        // NOTE: always include profiles for user
        builder.Entity<ApplicationUser>()
            .Navigation(x => x.Profiles)
            .AutoInclude();

        // Generate database entries for supported cultures

        foreach (var supportedCulture in SiteConstant.SupportedCultures)
        {
            // seed application languages
            builder.Entity<ApplicationLanguage>()
                .HasData(new ApplicationLanguage
                {
                    Id = supportedCulture.Key,
                    Culture = supportedCulture.Value
                });

            // seed administrator profiles
            builder.Entity<ApplicationUserProfile>()
                .HasData(new ApplicationUserProfile()
                {
                    LanguageId = supportedCulture.Key,
                    UserId = SiteConstant.ZeroGuid
                });
        }

        // Seed administrator

        builder.Entity<ApplicationUser>()
            .HasData(new ApplicationUser
            {
                Id = SiteConstant.ZeroGuid,
                ConcurrencyStamp = SiteConstant.ZeroGuid,
                SecurityStamp = SiteConstant.ZeroGuid,

                PasswordHash = "CHANGE_ME",

                UserName = SiteConstant.AdministratorUserName,
                NormalizedUserName = SiteConstant.AdministratorUserName.ToUpper(),
                Email = SiteConstant.AdministratorEmail,
                NormalizedEmail = SiteConstant.AdministratorEmail.ToUpper(),

                AvatarType = UserAvatarType.Gravatar,
                DisplayNameType = UserDisplayNameType.NamesAndUserName,

                DisplayEmail = true,
                EmailConfirmed = true,

                DateCreated = DateTime.Parse("2024-01-01"),
                LastUpdated = DateTime.Parse("2024-01-01"),

                PreferredLanguageId = SiteConstant.EnglishCulture.Key
            });
        
        // Seed roles
        
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
        
        // Grant admin to me
        
        builder.Entity<IdentityUserRole<string>>()
            .HasData(new IdentityUserRole<string>()
            {
                UserId = SiteConstant.ZeroGuid,
                RoleId = SiteConstant.ZeroGuid
            });

        // Convert enumerations to string

        builder.Entity<ApplicationDocument>()
            .Property(x => x.DocumentType)
            .HasConversion<string>();
        
        // we want these to be integers
        // builder.Entity<ApplicationDossier>()
        //     .Property(x => x.Status)
        //     .HasConversion<string>();

        builder.Entity<ApplicationUser>()
            .Property(x => x.AvatarType)
            .HasConversion<string>();

        // set collection conversions

        builder.Entity<DossierJournal>()
            .Property(p => p.MessageArguments)
            .HasConversion(array => string.Join(",", array),
                dbString => dbString.Split(",", StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(StringArrayValueComparer);

        builder.Entity<IssueJournal>()
            .Property(p => p.MessageArguments)
            .HasConversion(array => string.Join(",", array),
                dbString => dbString.Split(",", StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(StringArrayValueComparer);

        builder.Entity<SourceJournal>()
            .Property(p => p.MessageArguments)
            .HasConversion(array => string.Join(",", array),
                dbString => dbString.Split(",", StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(StringArrayValueComparer);

        // declare computed columns
        // NOTE: this is database specific (for MySQL)

        builder.Entity<ApplicationAuthor>(entity =>
        {
            entity.Property(e => e.Names)
                .HasComputedColumnSql(
                    $"CONCAT({HelperFunctions.ToSnakeCase(nameof(ApplicationAuthor.FirstName))}, " +
                    "' ', " +
                    $"{HelperFunctions.ToSnakeCase(nameof(ApplicationAuthor.LastName))})"
                );
        });

        // Configure one-to-one relationships

        builder.Entity<ApplicationSource>()
            .HasOne(source => source.Dossier)
            .WithOne(dossier => dossier.Source)
            .HasForeignKey<ApplicationSource>(source => source.DossierId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure many-to-many relationships
        // with dedicated mapping extension methods

        builder.Entity<ApplicationDocument>()
            .MapDossiersTable()
            .MapIssuesTable();
    }

    private static readonly ValueComparer StringArrayValueComparer = new ValueComparer<string[]>(
        (s1, s2) => s1.SequenceEqual(s2),
        strings => strings.Aggregate(0, (accumulator, value) => HashCode.Combine(accumulator, value.GetHashCode())),
        strings => strings.ToArray());
}
