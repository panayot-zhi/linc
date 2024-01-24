using linc.Utility;

namespace linc.Models.Enumerations;

public enum SiteRole
{
    Administrator = SiteRolesHelper.AdministratorWeight,
    HeadEditor = SiteRolesHelper.HeadEditorWeight,
    Editor = SiteRolesHelper.EditorWeight,
    UserPlus = SiteRolesHelper.UserPlusWeight,
    User = SiteRolesHelper.UserWeight
}
