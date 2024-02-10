using System.Security.Claims;
using linc.Contracts;
using linc.Data;
using linc.Models.Enumerations;

namespace linc.Utility
{
    public static class UserHelper
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static SiteRole GetRole(this ClaimsPrincipal user)
        {
            var roleName = user.FindFirst(ClaimTypes.Role)?.Value;

            if (Enum.TryParse(
                    value: roleName,
                    ignoreCase: true,
                    result: out SiteRole r))
            {
                return r;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static string GetDisplayName(this ApplicationUser user, ILocalizationService localizer)
        {
            string displayName;
            switch (user.DisplayNameType)
            {
                case UserDisplayNameType.UserName:
                    displayName = user.UserName;
                    break;
                case UserDisplayNameType.Names:
                    displayName = user.Names;
                    break;
                case UserDisplayNameType.Anonymous:
                    displayName = localizer["UserDisplayNameType_Anonymous"].Value;
                    break;
                case UserDisplayNameType.NamesAndUserName:
                    displayName = $"{user.Names} ({user.UserName})";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (user.DisplayEmail)
            {
                displayName += $" <{user.Email}>";
            }

            return displayName;
        }

    }
}
