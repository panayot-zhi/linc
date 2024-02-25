using System.ComponentModel.DataAnnotations;
using linc.Utility;

namespace linc.Models.Enumerations;

public enum SiteRole
{
    [Display(Name = "SiteRole_Administrator", ResourceType = typeof(Resources.SharedResource))]
    Administrator = SiteRolesHelper.AdministratorWeight,

    [Display(Name = "SiteRole_HeadEditor", ResourceType = typeof(Resources.SharedResource))]
    HeadEditor = SiteRolesHelper.HeadEditorWeight,

    [Display(Name = "SiteRole_Editor", ResourceType = typeof(Resources.SharedResource))]
    Editor = SiteRolesHelper.EditorWeight,

    [Display(Name = "SiteRole_UserPlus", ResourceType = typeof(Resources.SharedResource))]
    UserPlus = SiteRolesHelper.UserPlusWeight,

    [Display(Name = "SiteRole_User", ResourceType = typeof(Resources.SharedResource))]
    User = SiteRolesHelper.UserWeight
}
