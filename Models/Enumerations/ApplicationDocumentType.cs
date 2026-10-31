using System.ComponentModel.DataAnnotations;

namespace linc.Models.Enumerations;

public enum ApplicationDocumentType
{
    [Display(Name = "DocumentType_Unknown", ResourceType = typeof(Resources.SharedResource))]
    Unknown,

    [Display(Name = "DocumentType_CoverPage", ResourceType = typeof(Resources.SharedResource))]
    CoverPage,

    [Display(Name = "DocumentType_IndexPage", ResourceType = typeof(Resources.SharedResource))]
    IndexPage,


    [Display(Name = "DocumentType_IssuePdf", ResourceType = typeof(Resources.SharedResource))]
    IssuePdf,

    [Display(Name = "DocumentType_SourcePdf", ResourceType = typeof(Resources.SharedResource))]
    SourcePdf,


    [Display(Name = "DocumentType_Original", ResourceType = typeof(Resources.SharedResource))]
    Original,

    [Display(Name = "DocumentType_Anonymized", ResourceType = typeof(Resources.SharedResource))]
    Anonymized,

    [Display(Name = "DocumentType_Redacted", ResourceType = typeof(Resources.SharedResource))]
    Redacted,


    [Display(Name = "DocumentType_Review", ResourceType = typeof(Resources.SharedResource))]
    Review,

    [Display(Name = "DocumentType_SuperReview", ResourceType = typeof(Resources.SharedResource))]
    SuperReview,


    [Display(Name = "DocumentType_Agreement", ResourceType = typeof(Resources.SharedResource))]
    Agreement

}