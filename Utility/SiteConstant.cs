namespace linc.Utility;

public static class SiteConstant
{
    public const string True = nameof(True);
    public const string False = nameof(False);

    public const string IssuesFolderName = "issues";

    public const string DossiersFolderName = "dossiers";

    public const string TempDataEditableKey = "Editable";

    public const string PrintISSN = "3033-0181";

    public const string OnlineISSN = "3033-0599";

    public const string SiteName = "linc";

    public const string LandingPageArea = "LandingPage";

    // These should be never accessed outside
    // instead SupportedCulture dictionary should be used
    private const string EnglishLanguageShortName = "en";
    private const string BulgarianLanguageShortName = "bg";

    public static KeyValuePair<int, string> EnglishCulture => SupportedCultures
        .First(x => EnglishLanguageShortName.Equals(x.Value));

    public static KeyValuePair<int, string> BulgarianCulture => SupportedCultures
        .First(x => BulgarianLanguageShortName.Equals(x.Value));

    public static readonly Dictionary<int, string> SupportedCultures = new() {
        // NOTE: Key is database id
        // value is the culture
        {1, BulgarianLanguageShortName}, 
        {2, EnglishLanguageShortName}
    };

    public const string PublicationAgreementFileName = "publication_agreement.doc";
    public const string TechnicalRequirementsFileName = "technical_requirements.doc";
    public const string ZeroGuid = "00000000-0000-0000-0000-000000000000";

    public const string ReviewsEmail = "review-linc@uni-plovdiv.bg";
    public const string AdministratorEmail = "admin-linc@uni-plovdiv.bg";
    public const string AdministratorUserName = "p.ivanov";
    public const string AdministratorFirstName = "Panayot";
    public const string AdministratorLastName = "Ivanov";

    //public const string CyrillicNamePattern = "^[\\u0400-\\u04FF][\\u0400-\\u04FF-]+[\\u0400-\\u04FF]$";

    public const string LatinAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string CyrillicAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЬЮЯ";
    public static string AllowedUserNameLatinCharacters = LatinAlphabet.ToLowerInvariant() + LatinAlphabet.ToUpperInvariant();
    public static string AllowedUserNameCyrillicCharacters = CyrillicAlphabet.ToLowerInvariant() + CyrillicAlphabet.ToUpperInvariant();
    public const string AllowedUserNameNumberCharacters = "0123456789";
    public const string AllowedUserNameSpecialCharacters = "-._@ ";

    public static string AllowedUserNameCharacters = AllowedUserNameCyrillicCharacters +
                                                    AllowedUserNameLatinCharacters +
                                                    AllowedUserNameNumberCharacters +
                                                    AllowedUserNameSpecialCharacters;
}