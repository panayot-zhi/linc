using System.ComponentModel.DataAnnotations;
using linc.Models.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace linc.Data
{
    [Index(nameof(RelativePath), IsUnique = true)]
    public class ApplicationDocument
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(256)]
        public string OriginalFileName { get; set; }


        [Required]
        [MaxLength(127)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(32)]
        public string Extension { get; set; }

        [Required]
        [MaxLength(128)]
        public string MimeType { get; set; }


        [Required]
        [MaxLength(128)]
        public ApplicationDocumentType DocumentType { get; set; }


        [Required]
        [MaxLength(512)]
        public string RelativePath { get; set; }

        #region Navigation

        public ICollection<ApplicationSource> Sources { get; set; }

        public ICollection<ApplicationDossier> Dossiers { get; set; }

        public ICollection<ApplicationIssue> Issues { get; set; }

        #endregion Navigation

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
