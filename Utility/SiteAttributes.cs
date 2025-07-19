using linc.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace linc.Utility
{
    [Obsolete("Use 'fetch' instead.")]
    public class AjaxAttribute : ActionMethodSelectorAttribute
    {
        public string HttpVerb { get; set; }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.Request.IsAjax(HttpVerb);
        }
    }

    public class MinCountAttribute : ValidationAttribute
    {
        private readonly int _minCount;

        public MinCountAttribute(int minCount)
        {
            _minCount = minCount;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is ICollection collection && collection.Count >= _minCount)
            {
                return ValidationResult.Success;
            }

            // Use FormatErrorMessage to support resource-based error messages
            var errorMessage = string.Format(ErrorMessageString, validationContext.DisplayName, _minCount);
            return new ValidationResult(errorMessage);
        }
    }

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
