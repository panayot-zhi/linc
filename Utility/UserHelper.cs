using System.Security.Claims;
using linc.Contracts;
using linc.Data;
using linc.Models.Enumerations;

namespace linc.Utility
{
    public static class UserHelper
    {
        public const string ImpersonationClaimFlagKey = "IsImpersonating";
        public const string ImpersonationOriginalUserIdKey = "OriginalUserId";
        public const string ImpersonationOriginalUserNameKey = "OriginalUserName";

        public static bool IsImpersonating(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.HasClaim(ImpersonationClaimFlagKey, SiteConstant.True);
        }

        public static string GetOriginalUserId(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.FindFirst(ImpersonationOriginalUserIdKey)?.Value;
        }

        public static string GetOriginalUserName(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.FindFirst(ImpersonationOriginalUserNameKey)?.Value;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static SiteRole GetRole(this ClaimsPrincipal user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));
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
                    displayName = localizer["Anonymous_Label"].Value;
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
