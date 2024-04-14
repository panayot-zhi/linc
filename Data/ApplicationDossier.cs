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
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }


        [Required]
        public ApplicationDossierStatus Status { get; set; }


        #region Navigation

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public string CreatedById { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }


        [ForeignKey(nameof(AssignedTo))]
        public string AssignedToId { get; set; }

        public virtual ApplicationUser AssignedTo { get; set; }


        public virtual ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();

        public virtual ICollection<DossierJournal> Journals { get; set; } = new HashSet<DossierJournal>();

        #endregion Navigation

        #region NotMapped

        [NotMapped]
        public string Names => $"{FirstName} {LastName}";

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
        public ApplicationDocument Redacted =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Redacted);

        [NotMapped]
        public List<ApplicationDocument> Reviews =>
            Documents.Where(x => x.DocumentType == ApplicationDocumentType.Review).ToList();

        #endregion NotMapped

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion Automatic
    }
}
