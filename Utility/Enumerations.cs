namespace linc.Utility;

public enum UserAvatarType
{
    Default,    // Anonymous

    Gravatar,

    Facebook,
    Twitter,
    Google,

    Internal
}

public enum UserDisplayNameType
{
    UserName,
    Names,
    NamesAndUserName,
    Anonymous
}

public enum SiteRole
{
    Administrator = SiteRolesHelper.AdministratorWeight,
    HeadEditor = SiteRolesHelper.HeadEditorWeight,
    Editor = SiteRolesHelper.EditorWeight,
    UserPlus = SiteRolesHelper.UserPlusWeight,
    User = SiteRolesHelper.UserWeight
}
