using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace linc.Areas.Identity.Pages.Account.Manage
{
    [SiteAuthorize(SiteRole.Editor)]
    public class AdministrationModel : PageModel
    {

    }
}
