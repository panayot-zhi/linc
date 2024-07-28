using linc.Contracts;
using linc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace linc.Services;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    private readonly IApplicationUserStore _applicationUserStore;

    public ApplicationUserManager(IApplicationUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _applicationUserStore = store;
    }

    public virtual async Task<IdentityResult> UpdateUserProfiles(ApplicationUser user)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);
        await _applicationUserStore.UpdateUserProfiles(user);
        await UpdateSecurityStampAsync(user);
        return await UpdateUserAsync(user);
    }
}