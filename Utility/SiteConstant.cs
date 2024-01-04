namespace linc.Utility;

public static class SiteConstant
{
    public const string ZeroGuid = "00000000-0000-0000-0000-000000000000";

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