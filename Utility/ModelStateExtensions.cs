using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace linc.Utility
{
    public static class ModelStateExtensions
    {
        public static void AddIdentityError(this ModelStateDictionary modelState, IdentityError error)
        {
            modelState.AddModelError(error.Code, error.Description);
        }

        public static void AddIdentityErrors(this ModelStateDictionary modelState, IEnumerable<IdentityError> errors)
        {
            foreach (var identityError in errors)
            {
                modelState.AddIdentityError(identityError);
            }
        }
    }
}
