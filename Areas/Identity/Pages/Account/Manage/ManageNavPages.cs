// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Mvc.Rendering;

namespace  linc.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public const string Index = nameof(Index);

        public const string Information = nameof(Information);

        public const string Avatar = nameof(Avatar);

        public const string Email = nameof(Email);

        public const string ChangePassword = nameof(ChangePassword);

        public const string DownloadPersonalData = nameof(DownloadPersonalData);

        public const string DeletePersonalData = nameof(DeletePersonalData);

        public const string ExternalLogins = nameof(ExternalLogins);

        public const string PersonalData = nameof(PersonalData);

        public const string TwoFactorAuthentication = nameof(TwoFactorAuthentication);

        public const string Administration = nameof(Administration);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
