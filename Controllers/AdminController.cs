using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.ViewModels.Emails;
using System.Globalization;
using linc.Contracts;

namespace linc.Controllers
{
    public class AdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISiteEmailSender _emailSender;

        public AdminController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILocalizationService localizationService, 
            ISiteEmailSender emailSender)
        : base(localizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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

        public async Task<IActionResult> TestSendEmail(string id)
        {
            var request = HttpContext.Request;
            var domainUrl = $"{request.Scheme}://{request.Host}";

            var viewModel = new TestEmail()
            {
                Test = LocalizationService["Logo_Long"].Value,
                TestButton = new EmailButton
                {
                    Text = LocalizationService["Logo_Short"].Value,
                    Url = domainUrl
                }
            };

            if (!string.IsNullOrEmpty(id))
            {
                // test language display
                viewModel.Language = new CultureInfo(id).Name;
            }

            var email = new SiteEmailDescriptor<TestEmail>()
            {
                Emails = new()
                {
                    SiteConstant.AdministratorEmail
                },
                Subject = "Коле, получи ли?",
                ViewModel = viewModel
            };

            await _emailSender.SendEmailAsync(email);

            //AddAlertMessage("Sent");

            return View($"Emails/TestEmail.{viewModel.Language}", viewModel);
        }
    }
}
