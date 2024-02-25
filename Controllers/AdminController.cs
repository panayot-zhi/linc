using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using linc.Data;

namespace linc.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            var currentUserId = User.GetUserId();
            var currentUserName = User.GetUserName();

            var impersonatedUser = await _userManager.FindByIdAsync(userId);

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(impersonatedUser);

            var impersonationClaims = new List<Claim>()
            {
                new(UserHelper.ImpersonationOriginalUserIdKey, currentUserId),
                new(UserHelper.ImpersonationOriginalUserNameKey, currentUserName),
                new(UserHelper.ImpersonationClaimFlagKey, SiteConstant.True)
            };

            var identity = userPrincipal.Identities.First();
            identity.AddClaims(impersonationClaims);

            // sign out the current user
            await _signInManager.SignOutAsync();

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal);

            return RedirectToAction("Index", "Home");
        }

        [SiteAuthorize]
        public async Task<IActionResult> StopImpersonation()
        {
            if (!User.IsImpersonating())
            {
                throw new Exception("You are not impersonating now. Can't stop impersonation");
            }

            var originalUserId = User.GetOriginalUserId();

            var originalUser = await _userManager.FindByIdAsync(originalUserId);

            await _signInManager.SignOutAsync();

            await _signInManager.SignInAsync(originalUser, isPersistent: true);

            return RedirectToAction("Index", "Home");
        }
    }
}
