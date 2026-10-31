using linc.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;

namespace linc.Utility
{
    // NOTE: ~as class authorization is not easily overridable, we mark actions only~
    // NOTE: but, since page models are classes we must allow the attribute targets to protect them
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SiteAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Requires one to be logged in.
        /// </summary>
        public SiteAuthorizeAttribute()
        {

        }

        /// <summary>
        /// Requires one to have the specified role OR (default) be higher than it.
        /// Pass flag andAbove as false to change the default behaviour to authorize for specific role only. 
        /// </summary>
        public SiteAuthorizeAttribute(SiteRole role, bool andAbove = true)
        {
            var roles = SiteRolesHelper.GetRoleName(role);

            if (andAbove)
            {
                roles = string.Join(",", SiteRolesHelper.GetRoleNamesAbove(role));
            }

            this.Roles = roles;
        }
    }
}
