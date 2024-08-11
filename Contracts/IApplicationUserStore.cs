using linc.Data;
using Microsoft.AspNetCore.Identity;

namespace linc.Contracts
{
    public interface IApplicationUserStore : IUserStore<ApplicationUser>
    {
        Task UpdateUserProfiles(ApplicationUser user);
    }
}
