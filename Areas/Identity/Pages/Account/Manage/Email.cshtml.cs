// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmailModel> _logger;
        private readonly ISiteEmailSender _emailSender;

        public EmailModel(
            ILogger<EmailModel> logger,
            UserManager<ApplicationUser> userManager,
            ILocalizationService localizationService,
            ISiteEmailSender emailSender)
        : base(localizationService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [Display(Name = "EmailModel_Email", ResourceType = typeof(Resources.SharedResource))]
        public string Email { get; private set; }

        public bool IsEmailConfirmed { get; private set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "EmailModel_NewEmail", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            Input = new InputModel();

            Email = await _userManager.GetEmailAsync(user);

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to find user with ID {UserId}",
                    _userManager.GetUserId(User));
                return NotFound();
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to find user with ID {UserId}",
                    _userManager.GetUserId(User));
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId, email = Input.NewEmail, code },
                    protocol: Request.Scheme);

                ArgumentNullException.ThrowIfNull(callbackUrl);

                var model = new SiteEmailDescriptor<ConfirmEmailChange>()
                {
                    Emails = new() { Input.NewEmail },
                    Subject = LocalizationService["Email_ConfirmEmailChange_Subject"].Value,
                    ViewModel = new ConfirmEmailChange
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

                StatusMessage = SuccessStatusMessage(
                    LocalizationService["ManageEmail_ChangeEmail_SuccessMessage"]
                );
                return RedirectToPage();
            }

            StatusMessage = WarningStatusMessage(
                LocalizationService["ManageEmail_ChangeEmail_WarningMessage"]
            );

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to find user with ID {UserId}",
                    _userManager.GetUserId(User));
                return NotFound();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
                protocol: Request.Scheme);

            ArgumentNullException.ThrowIfNull(callbackUrl);

            var model = new SiteEmailDescriptor<ConfirmEmail>()
            {
                Emails = new() { email },
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

            _logger.LogInformation("Resending confirmation email for un-verified, but logged in user: {UserId}",
                userId);

            await _emailSender.SendEmailAsync(model);

            StatusMessage = SuccessStatusMessage(
                LocalizationService["ManageEmail_ReSendVerificationEmail_SuccessMessage"]
            );

            return RedirectToPage();
        }
    }
}
