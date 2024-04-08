using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using linc.Models.Enumerations;

namespace linc.Data
{
    public class ApplicationDocument
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(256)]
        public string Title { get; set; }


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


        [NotMapped]
        [JsonIgnore]
        public byte[] Content { get; set; }


        public ApplicationSource Source { get; set; }

        public ICollection<ApplicationDossier> Dossiers { get; set; }

        public ICollection<ApplicationIssue> Issues { get; set; }


        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
