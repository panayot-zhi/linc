namespace linc.Models.Enumerations;

public enum ApplicationDossierStatus
{
    New = 0,

    Prepared = 10,
    InReview = 20,
    Reviewed = 25,

    Accepted = 50,
    AcceptedWithCorrections = 55,
    AwaitingCorrections = 70,

    Rejected = 90
}