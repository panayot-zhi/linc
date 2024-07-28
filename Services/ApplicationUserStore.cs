using linc.Contracts;
using linc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace linc.Services;

public class ApplicationUserStore : UserStore<ApplicationUser>, IApplicationUserStore
{
    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
    {
    }

    public async Task UpdateUserProfiles(ApplicationUser user, ApplicationUserProfile[] profiles)
    {
        foreach (var profile in profiles)
        {
            var userProfile = user.Profiles.First(x =>
                x.UserId == user.Id && x.LanguageId == profile.LanguageId);

            Context.Attach(userProfile);

            if (userProfile.FirstName != profile.FirstName)
                userProfile.FirstName = profile.FirstName;

            if (userProfile.LastName != profile.LastName)
                userProfile.LastName = profile.LastName;

            if (userProfile.Description != profile.Description)
                userProfile.Description = profile.Description;

            Context.Update(userProfile);
        }

        await Context.SaveChangesAsync();
    }
}