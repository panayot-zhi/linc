// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using linc.Contracts;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace linc.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class LockoutModel : BasePageModel
    {
        public LockoutModel(ILocalizationService localizer) 
            : base(localizer)
        {
        }

        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IActionResult OnGet()
        {
            StatusMessage = LocalizationService["Lockout_WarningMessage"].Value;
            AddAlertMessage(LocalizationService["Lockout_WarningMessage"],
                type: AlertMessageType.Warning);

            return Redirect("/");
        }
    }
}
