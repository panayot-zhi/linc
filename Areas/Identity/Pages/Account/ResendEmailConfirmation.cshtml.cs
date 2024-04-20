// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResendEmailConfirmationModel> _logger;
        private readonly ISiteEmailSender _emailSender;

        public ResendEmailConfirmationModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ResendEmailConfirmationModel> logger,
            ILocalizationService localizationService,
            ISiteEmailSender emailSender)
        : base(localizationService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "RegisterModel_Email", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string Email { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                _logger.LogWarning("An attempt was made to resend confirmation email for a non-existent user: {UserInput}",
                    Input.Email);

                // Don't reveal that the user does not exist

                AddAlertMessage(LocalizationService["ResendEmailConfirmation_SuccessMessage"],
                    type: AlertMessageType.Success);

                return Redirect("/");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, code },
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

            _logger.LogWarning("Resending confirmation email for un-verified user: {UserId}",
                userId);

            await _emailSender.SendEmailAsync(model);

            AddAlertMessage(LocalizationService["ResendEmailConfirmation_SuccessMessage"],
                type: AlertMessageType.Success);

            return Redirect("/");
        }
    }
}
