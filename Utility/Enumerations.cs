using System.ComponentModel.DataAnnotations;

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
    [Display(Name = "Псевдоним")]
    UserName,

    [Display(Name = "Имена")]
    Names,

    [Display(Name = "Имена и псевдоним")]
    NamesAndUserName,

    [Display(Name = "Да се показва само 'Анонимен'")]
    Anonymous
}

public enum SiteRole
{
    [Display(Name = "Администратор")]
    Administrator = SiteRolesHelper.AdministratorWeight,

    [Display(Name = "Главен редактор")]
    HeadEditor = SiteRolesHelper.HeadEditorWeight,

    [Display(Name = "Редактор")]
    Editor = SiteRolesHelper.EditorWeight,

    [Display(Name = "Потребител +")]
    UserPlus = SiteRolesHelper.UserPlusWeight,

    [Display(Name = "Потребител")]
    User = SiteRolesHelper.UserWeight
}
