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
        public int ReleaseYear { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }


        public ICollection<ApplicationSource> Sources { get; set; } = new List<ApplicationSource>();

        public ICollection<ApplicationDocument> Files { get; set; } = new List<ApplicationDocument>();


        [NotMapped]
        public List<ApplicationDocument> IndexPages =>
            Files.Where(x => x.DocumentType == ApplicationDocumentType.IndexPage).ToList();

        [NotMapped]
        public ApplicationDocument CoverPage =>
            Files.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.CoverPage);

        [NotMapped]
        public ApplicationDocument Pdf =>
            Files.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.IssuePdf);


        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
