namespace linc.Utility;

public static class Constants
{
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