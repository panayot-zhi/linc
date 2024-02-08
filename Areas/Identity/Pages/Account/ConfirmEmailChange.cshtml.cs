// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Text;
using linc.Contracts;
using linc.Data;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ConfirmEmailChangeModel> _logger;

        public ConfirmEmailChangeModel(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ILogger<ConfirmEmailChangeModel> logger,
            ILocalizationService localizer)
        : base(localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public string StatusMessage { get; set; }
         
        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                _logger.LogWarning("{MethodName} received insufficient parameters: {@Parameters}",
                    nameof(OnGetAsync), new[] { userId, email, code });

                return Redirect("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm user's '{UserId}' email address ({Email}) change with code [{Code}]: User not found",
                    userId, email, code);

                return NotFound();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (result.Succeeded)
            {
                StatusMessage = LocalizationService["ConfirmEmailChange_SuccessMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmailChange_SuccessMessage"],
                    type: AlertMessageType.Success);
            }
            else
            {
                _logger.LogError("Failed to confirm user's '{UserId}' email address ({Email}) change with code [{Code}]: {ResultErrors}",
                    userId, email, code, string.Join(",", result.Errors.Select(x => $"{x.Code} - {x.Description}")));
                StatusMessage = LocalizationService["ConfirmEmailChange_ErrorMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmailChange_ErrorMessage"],
                    type: AlertMessageType.Error);
            }

            await _signInManager.RefreshSignInAsync(user);

            // Don't use the page
            // return Page();

            return Redirect("/");
        }
    }
}
