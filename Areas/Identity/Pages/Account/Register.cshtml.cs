// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using linc.Data;
using linc.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
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
            [Display(Name = "Име")]
            [Required(ErrorMessage = "Моля, въведете име.")]
            [MaxLength(255, ErrorMessage = "Въведеното име надвишава позволеният размер ({1}).")]
            // [RegularExpression(SiteConstant.CyrillicNamePattern, ErrorMessage = "Моля, въведете име на кирилица.")]
            public string FirstName { get; set; }

            [Display(Name = "Фамилия")]
            [Required(ErrorMessage = "Моля, въведете фамилия.")]
            [MaxLength(255, ErrorMessage = "Въведеното име надвишава позволеният размер ({1}).")]
            // [RegularExpression(SiteConstant.CyrillicNamePattern, ErrorMessage = "Моля, въведете име на кирилица.")]
            public string LastName { get; set; }

            [Display(Name = "Псевдоним")]
            [MaxLength(126, ErrorMessage = "Въведеният псевдоним надвишава позволеният размер ({1}).")]
            [Required(ErrorMessage = "Моля, въведете псевдоним.")]
            public string UserName { get; set; }

            [Display(Name = "Email")]
            [Required(ErrorMessage = "Моля, въведете email адрес.")]
            [EmailAddress(ErrorMessage = "Моля, въведете валиден email адрес.")]
            [MaxLength(255, ErrorMessage = "Въведеният email надвишава позволеният размер ({1}).")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Моля, въведете парола.")]
            [StringLength(100, ErrorMessage = "Полето за {0} трябва да е с поне {2} и най-много {1} символа дължина.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Потвърди")]
            [Compare("Password", ErrorMessage = "Двете пароли не съвпадат.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "За да се регистрирате се изисква да сте прочели и да приемате общите условия на сайта и политиката му за поверителност.")]
            [Display(Name = "Прочетох и приемам Политиката за поверителност на сайта и Общите му условия")]
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
                // TODO: i18n
                ModelState.AddModelError(string.Empty, "За да се регистрирате се изисква да сте прочели и да приемате общите условия на сайта и политиката му за поверителност.");
            }

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(ReturnUrl); 
            }

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
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
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = ReturnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = ReturnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(ReturnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
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
