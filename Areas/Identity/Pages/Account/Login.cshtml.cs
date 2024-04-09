// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using linc.Contracts;
using linc.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace linc.Areas.Identity.Pages.Account
{
    public class LoginModel : BasePageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;


        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILocalizationService localizationService,
            ILogger<LoginModel> logger)
        : base(localizationService)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [Display(Name = "LoginModel_UserName", ResourceType = typeof(Resources.SharedResource))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [Display(Name = "LoginModel_Password", ResourceType = typeof(Resources.SharedResource))]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "LoginModel_RememberMe", ResourceType = typeof(Resources.SharedResource))]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            if (User.Identity is { IsAuthenticated: true })
            {
                return LocalRedirect(returnUrl);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                // validation failed, 
                // redisplay form
                return Page();
            }

            SignInResult result;
            var user = await GetUserFromInputAsync(Input.UserName);
            if (user == null)
            {
                result = SignInResult.Failed;
            }
            else
            {
                result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            }

            if (result.Succeeded)
            {
                // ReSharper disable once PossibleNullReferenceException
                _logger.LogInformation("User {UserName} logged in.", user.UserName);

                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                // ReSharper disable once PossibleNullReferenceException
                _logger.LogWarning("User '{UserName}' account locked out.", user.UserName);
                return RedirectToPage("./Lockout");
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, LocalizationService["Register_EmailConfirmation_Required"].Value);
                return Page();
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, LocalizationService["Login_InvalidLoginAttempt"].Value);
            return Page();
        }

        private async Task<ApplicationUser> GetUserFromInputAsync(string input)
        {
            var userManager = _signInManager.UserManager;
            var byName = await userManager.FindByNameAsync(input);
            if (byName != null)
            {
                _logger.LogInformation("User {Username} recognized by username.", byName.UserName);
                return byName;
            }

            var byEmail = await userManager.FindByEmailAsync(input);
            if (byEmail != null)
            {
                _logger.LogInformation("User {User} recognized by email ({Email}).", byEmail.UserName, byEmail.Email);
                return byEmail;
            }

            _logger.LogWarning("User cannot be recognized neither by username nor by email.");

            return null;
        }
    }
}
