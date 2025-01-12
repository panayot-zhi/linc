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
        private readonly IDossierService _dossierService;
        private readonly ISourceService _sourceService;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, 
            ILogger<ConfirmEmailModel> logger, 
            ILocalizationService localizationService,
            IDossierService dossierService,
            ISourceService sourcesService)
        : base(localizationService)
        {
            _logger = logger;
            _sourceService = sourcesService;
            _dossierService = dossierService;
            _userManager = userManager;
        }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogWarning("{MethodName} received insufficient parameters: {@Parameters}", 
                    nameof(OnGetAsync), new[] { userId, code });

                return Redirect("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to confirm user {UserId} email address with code [{Code}]: User not found",
                    userId, code);

                return NotFound();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // TODO: make asynchronous
                await Sync(user);

                StatusMessage = LocalizationService["ConfirmEmail_SuccessMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmail_SuccessMessage"],
                    type: AlertMessageType.Success);
            }
            else
            {
                _logger.LogError("Failed to confirm user {UserId} email address with code [{Code}]: {ResultErrors}",
                    userId, code, string.Join(",", result.Errors.Select(x => $"{x.Code} - {x.Description}")));
                StatusMessage = LocalizationService["ConfirmEmail_ErrorMessage"].Value;
                AddAlertMessage(LocalizationService["ConfirmEmail_ErrorMessage"],
                    type: AlertMessageType.Error);
            }

            // Don't use the page
            // return Page();

            return RedirectToPage("./Login");
        }

        private async Task Sync(ApplicationUser user)
        {
            await _sourceService.UpdateAuthorAsync(user);
            await _dossierService.UpdateAuthorAsync(user);
            await _dossierService.UpdateReviewerAsync(user);
        }
    }
}
