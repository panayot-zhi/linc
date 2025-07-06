using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using linc.Models.Enumerations;

namespace linc.Data
{
    public class ApplicationIssue
    {
        public int Id { get; set; }

        public bool IsAvailable { get; set; }


        [Required]
        public int IssueNumber { get; set; }

        [Required]
        public DateOnly ReleaseDate { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }


        #region Navigation

        public ICollection<ApplicationSource> Sources { get; set; } = new List<ApplicationSource>();

        public ICollection<ApplicationDocument> Documents { get; set; } = new List<ApplicationDocument>();

        #endregion Navigation

        #region NotMapped

        [NotMapped]
        public int ReleaseYear => ReleaseDate.Year;

        [NotMapped]
        public List<ApplicationDocument> IndexPages =>
            Documents.Where(x => x.DocumentType == ApplicationDocumentType.IndexPage).ToList();

        [NotMapped]
        public ApplicationDocument CoverPage =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.CoverPage);

        [NotMapped]
        public ApplicationDocument Pdf =>
            Documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.IssuePdf);

        #endregion NotMapped

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
