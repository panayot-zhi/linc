using System.ComponentModel.DataAnnotations;

namespace linc.Models.Enumerations;

public enum UserDisplayNameType
{
    [Display(Name = "UserDisplayNameType_UserName", ResourceType = typeof(Resources.SharedResource))]
    UserName,

    [Display(Name = "UserDisplayNameType_Names", ResourceType = typeof(Resources.SharedResource))]
    Names,

    [Display(Name = "UserDisplayNameType_NamesAndUserName", ResourceType = typeof(Resources.SharedResource))]
    NamesAndUserName,

    [Display(Name = "UserDisplayNameType_Anonymous", ResourceType = typeof(Resources.SharedResource))]
    Anonymous
}