// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Emails;
using linc.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : BasePageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ISiteEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            ILocalizationService localizationService,
            ISiteEmailSender emailSender,
            ILogger<RegisterModel> logger)
        : base(localizationService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Display(Name = "RegisterModel_FirstName", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string FirstName { get; set; }

            [Display(Name = "RegisterModel_LastName", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string LastName { get; set; }

            [Display(Name = "RegisterModel_UserName", ResourceType = typeof(Resources.SharedResource))]
            [MaxLength(127, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string UserName { get; set; }

            [Display(Name = "RegisterModel_Email", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "RegisterModel_Password", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "StringLengthAttribute_ValidationErrorIncludingMinimum", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "RegisterModel_ConfirmPassword", ResourceType = typeof(Resources.SharedResource))]
            [Compare(nameof(Password), ErrorMessageResourceName = "CompareAttribute_MustMatch", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [Display(Name = "RegisterModel_PrivacyConsent", ResourceType = typeof(Resources.SharedResource))]
            public bool PrivacyConsent { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (User.Identity is { IsAuthenticated: true })
            {
                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (Input.PrivacyConsent != true)
            {
                ModelState.AddModelError(string.Empty, LocalizationService["RegisterModel_PrivacyConsent_ErrorMessage"].Value);
            }

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(ReturnUrl); 
            }

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // something failed,
                // redisplay form
                return Page();
            }

            var defaultRole = await _roleManager.FindByNameAsync(SiteRolesHelper.UserRoleName);

            if (defaultRole != null)
            {
                await _userManager.AddToRoleAsync(user, defaultRole.Name);
            }

            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl = ReturnUrl },
                protocol: Request.Scheme);

            ArgumentNullException.ThrowIfNull(callbackUrl);

            var model = new SiteEmailDescriptor<ConfirmEmail>()
            {
                Emails = new() { Input.Email },
                Subject = LocalizationService["Email_ConfirmEmail_Subject"].Value,
                ViewModel = new ConfirmEmail
                {
                    Names = user.Names,
                    Confirm = new LinkViewModel
                    {
                        Url = callbackUrl,
                        Text = LocalizationService["Email_ConfirmEmail_ConfirmButton_Label"].Value
                    }
                }
            };

            await _emailSender.SendEmailAsync(model);

            AddAlertMessage(LocalizationService["RegisterConfirmation_InfoMessage"],
                type: AlertMessageType.Success);

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return Redirect("/");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(ReturnUrl);
        }

        private ApplicationUser CreateUser()
        {
            return new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.UserName,
                Email = Input.Email
            };

        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
