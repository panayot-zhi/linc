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
        [MaxLength(128)]
        public ApplicationDossierStatus Status { get; set; }


        public ICollection<ApplicationDocument> Documents { get; set; }


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
    }
}
