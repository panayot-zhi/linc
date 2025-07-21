using linc.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data
{
    public class ApplicationDossier
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(1024)]
        public string Title { get; set; }


        [Required]
        public ApplicationDossierStatus Status { get; set; }

        public bool SuperReviewed { get; set; }


        #region Navigation

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public string CreatedById { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }


        [ForeignKey(nameof(AssignedTo))]
        public string AssignedToId { get; set; }

        public virtual ApplicationUser AssignedTo { get; set; }


        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public virtual ApplicationLanguage Language { get; set; }

        // NOTE: no foreign key here, because the relationship
        // is controlled by the ApplicationSource entity
        public ApplicationSource Source { get; set; }


        public virtual ICollection<ApplicationDossierReview> Reviews { get; set; } = new List<ApplicationDossierReview>();

        public virtual ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();

        public virtual ICollection<ApplicationAuthor> Authors { get; set; } = new List<ApplicationAuthor>();

        public virtual ICollection<DossierJournal> Journals { get; set; } = new HashSet<DossierJournal>();

        #endregion Navigation

        #region NotMapped

        [NotMapped]
        public ApplicationDocument Original =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Original);

        [NotMapped]
        public ApplicationDocument Anonymized =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Anonymized);

        [NotMapped]
        public ApplicationDocument Agreement =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Agreement);

        [NotMapped]
        public bool HasAgreement => Agreement != null;

        [NotMapped]
        public ApplicationDocument Redacted =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Redacted);

        #endregion NotMapped

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion Automatic
    }
}
