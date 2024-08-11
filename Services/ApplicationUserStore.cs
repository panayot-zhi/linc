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

    public async Task UpdateUserProfiles(ApplicationUser user)
    {
        foreach (var profile in user.Profiles)
        {
            Context.Attach(profile);
        }

        await Context.SaveChangesAsync();
    }
}