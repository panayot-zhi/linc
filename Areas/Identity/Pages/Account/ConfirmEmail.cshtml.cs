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
    public class ConfirmEmailModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, 
            ILogger<ConfirmEmailModel> logger, 
            ILocalizationService localizer)
        : base(localizer)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return Redirect("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm user '{UserId}' email address with code [{Code}]: User not found",
                    userId, code);

                return NotFound();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                StatusMessage = LocalizationService["ConfirmEmail_SuccessMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmail_SuccessMessage"],
                    type: AlertMessageType.Success);
            }
            else
            {
                _logger.LogError("Failed to confirm user '{UserId}' email address with code [{Code}]: {ResultErrors}",
                    userId, code, string.Join(",", result.Errors.Select(x => $"{x.Code} - {x.Description}")));
                StatusMessage = LocalizationService["ConfirmEmail_ErrorMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmail_ErrorMessage"],
                    type: AlertMessageType.Error);
            }

            return Page();

            return Redirect("/");
        }
    }
}
