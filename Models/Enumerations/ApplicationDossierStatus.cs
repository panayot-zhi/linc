namespace linc.Models.Enumerations;

public enum ApplicationDossierStatus
{
    New,

    Prepared,
    InReview,
    Reviewed,

    Accepted,
    Rejected,
    AcceptedWithCorrections,
    AwaitingCorrections
}