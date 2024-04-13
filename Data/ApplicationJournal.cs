using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using linc.Models.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace linc.Data
{
    [Index(nameof(Message), IsUnique = false)]
    public abstract class ApplicationJournal
    {
        public int Id { get; set; }

        public ApplicationJournalType Type { get; set; }

        [MaxLength(512)]
        public string Message { get; set; }


        [Required]
        [ForeignKey(nameof(PerformedBy))]
        public string PerformedById { get; set; }

        public ApplicationUser PerformedBy { get; set; }


        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }

    public class DossierJournal : ApplicationJournal
    {
        [Required]
        [ForeignKey(nameof(Dossier))]
        public int DossierId { get; set; }

        public ApplicationDossier Dossier { get; set; }
    }

    public class SourceJournal : ApplicationJournal
    {
        [Required]
        [ForeignKey(nameof(Source))]
        public int SourceId { get; set; }

        public ApplicationSource Source { get; set; }
    }

    public class IssueJournal : ApplicationJournal
    {
        [Required]
        [ForeignKey(nameof(Issue))]
        public int IssueId { get; set; }

        public ApplicationIssue Issue { get; set; }
    }
}
