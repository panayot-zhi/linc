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
using linc.Models.ViewModels;

namespace linc.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISiteEmailSender _emailSender;
        private readonly IAuthorService _authorService;

        public AdminController(
            ILogger<AdminController> logger,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILocalizationService localizationService, 
            ISiteEmailSender emailSender, 
            IAuthorService authorService)
        : base(localizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _authorService = authorService;
            _logger = logger;
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

        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> SyncUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            await Sync(user);

            return Ok("User synchronized successfully.");
        }

        [SiteAuthorize]
        public async Task<IActionResult> StopImpersonation()
        {
            if (!User.IsImpersonating())
            {
                _logger.LogError("User '{UserName}' ({UseId}) is not currently impersonating. Can't stop impersonation: continuing",
                    User.GetUserName(), User.GetUserId());

                //throw new Exception("You are not impersonating now. Can't stop impersonation");

                return Redirect("/");
            }

            var originalUserId = User.GetOriginalUserId();

            var originalUser = await _userManager.FindByIdAsync(originalUserId);

            await _signInManager.SignOutAsync();

            await _signInManager.SignInAsync(originalUser, isPersistent: true);

            return Redirect("/");
        }

        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> TestSendEmail(string id)
        {
            var request = HttpContext.Request;
            var domainUrl = $"{request.Scheme}://{request.Host}";

            var viewModel = new TestEmail()
            {
                Test = LocalizationService["Logo_Long"].Value,
                TestButton = new LinkViewModel
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

        private async Task Sync(ApplicationUser user)
        {
            await _authorService.UpdateAuthorsUserAsync(user);
        }
    }
}
