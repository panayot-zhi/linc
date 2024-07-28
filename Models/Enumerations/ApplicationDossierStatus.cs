using System.ComponentModel.DataAnnotations;

namespace linc.Models.Enumerations;

public enum ApplicationDossierStatus
{
    [Display(Name = "DossierStatus_New", ResourceType = typeof(Resources.SharedResource))]
    New = 0,

    [Display(Name = "DossierStatus_Prepared", ResourceType = typeof(Resources.SharedResource))]
    Prepared = 10,

    [Display(Name = "DossierStatus_InReview", ResourceType = typeof(Resources.SharedResource))]
    InReview = 20,

    [Display(Name = "DossierStatus_Reviewed", ResourceType = typeof(Resources.SharedResource))]
    Reviewed = 25,


    [Display(Name = "DossierStatus_AwaitingCorrections", ResourceType = typeof(Resources.SharedResource))]
    AwaitingCorrections = 40,


    [Display(Name = "DossierStatus_Accepted", ResourceType = typeof(Resources.SharedResource))]
    Accepted = 50,

    [Display(Name = "DossierStatus_AcceptedWithCorrections", ResourceType = typeof(Resources.SharedResource))]
    AcceptedWithCorrections = 55,


    [Display(Name = "DossierStatus_Published", ResourceType = typeof(Resources.SharedResource))]
    Published = 60,


    [Display(Name = "DossierStatus_Rejected", ResourceType = typeof(Resources.SharedResource))]
    Rejected = 90
}