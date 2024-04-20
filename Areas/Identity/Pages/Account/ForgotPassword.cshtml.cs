// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Emails;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly ISiteEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, 
            ISiteEmailSender emailSender, 
            ILogger<ForgotPasswordModel> logger, 
            ILocalizationService localizationService)
        : base(localizationService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "RegisterModel_Email", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogWarning("An attempt was made to restore a forgotten password for a {ErrorType:l} user with email {Email}.",
                    user == null ? "non-existent" : "not confirmed", Input.Email);

                // Don't reveal that the user does not exist or is not confirmed

                AddAlertMessage(LocalizationService["ForgotPassword_InfoMessage"],
                    type: AlertMessageType.Info);

                return Redirect("/");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            ArgumentNullException.ThrowIfNull(callbackUrl);

            var model = new SiteEmailDescriptor<ResetPassword>()
            {
                Emails = new() { Input.Email },
                Subject = LocalizationService["Email_ResetPassword_Subject"].Value,
                ViewModel = new ResetPassword
                {
                    IpAddress = HelperFunctions.GetIp(HttpContext),
                    Reset = new LinkViewModel
                    {
                        Url = callbackUrl,
                        Text = LocalizationService["Email_ResetPassword_ResetButton_Label"].Value
                    }
                }
            };

            await _emailSender.SendEmailAsync(model);

            AddAlertMessage(LocalizationService["ForgotPassword_InfoMessage"],
                type: AlertMessageType.Info);

            return Redirect("/");
        }
    }
}
