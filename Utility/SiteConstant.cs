using System.Collections.ObjectModel;

namespace linc.Utility;

public static class SiteConstant
{
    public static readonly Dictionary<int, string> SupportedCultures = new() {
        // NOTE: Key is database id
        // value is the culture
        {1, "bg"}, 
        {2, "en"}
    };

    public const string ZeroGuid = "00000000-0000-0000-0000-000000000000";

    public const string ReviewsEmail = "review-linc@uni-plovdiv.bg";
    public const string AdministratorEmail = "admin-linc@uni-plovdiv.bg";
    public const string AdministratorUserName = "p.ivanov";
    public const string AdministratorFirstName = "Panayot";
    public const string AdministratorLastName = "Ivanov";

    public const string CyrillicNamePattern = "^[\\u0400-\\u04FF][\\u0400-\\u04FF-]+[\\u0400-\\u04FF]$";

    public const string AllowedUserNameLatinCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AllowedUserNameCyrillicCharacters = "абвгдежзийклмнопрстуфхцчшщъьюяАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
    public const string AllowedUserNameNumberCharacters = "0123456789";
    public const string AllowedUserNameSpecialCharacters = "-._@ ";

    public const string AllowedUserNameCharacters = AllowedUserNameCyrillicCharacters +
                                                    AllowedUserNameLatinCharacters +
                                                    AllowedUserNameNumberCharacters +
                                                    AllowedUserNameSpecialCharacters;
}