using linc.Contracts;
using linc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace linc.Services;

public class ApplicationUserStore : UserStore<ApplicationUser>, IApplicationUserStore
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
    {
        _context = context;
    }

    public async Task UpdateUserProfiles(ApplicationUser user)
    {
        foreach (var profile in user.Profiles)
        {
            Context.Attach(profile);
        }

        await Context.SaveChangesAsync();
    }

    public async Task<ApplicationUser> FindUserByEmailAsync(string inputEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == inputEmail);
    }
        
    public async Task<ApplicationUser> FindUserByNamesAsync(string inputFirstName, string inputLastName)
    {
        // trim input names
        inputFirstName = inputFirstName?.Trim();
        inputLastName = inputLastName?.Trim();

        var userProfiles = await _context.UserProfiles.AsTracking()
            .Include(x => x.User)
            .Where(x =>
                x.FirstName.Equals(inputFirstName) &&
                x.LastName.Equals(inputLastName)
            )
            .ToArrayAsync();

        // if we have users that filled both names the same way for all profiles
        userProfiles = userProfiles
            .DistinctBy(x => x.UserId)
            .ToArray();

        if (!userProfiles.Any())
        {
            return null;
        }

        if (userProfiles.Length > 1)
        {
            Log.Logger.ForContext<ApplicationUserStore>().Warning("FindReviewerByNamesAsync found more than 1 match for the reviewer with names {Names} and will not assign user.",
                $"{inputFirstName} {inputLastName}");
            return null;
        }

        return userProfiles.First().User;
    }
}