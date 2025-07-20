using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace linc.Data
{
    [Index(nameof(PdfId), IsUnique = false)]
    [Index(nameof(FirstName), IsUnique = false)]
    [Index(nameof(LastName), IsUnique = false)]
    [Index(nameof(AuthorNames), IsUnique = false)]
    // [Index(nameof(AuthorNotes), IsUnique = false)]
    [Index(nameof(Title), IsUnique = false)]
    // [Index(nameof(TitleNotes), IsUnique = false)]
    public class ApplicationSource
    {
        public int Id { get; set; }


        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(512)]
        public string AuthorNames { get; init; }

        [MaxLength(1024)]
        public string AuthorNotes { get; set; }

        /// <summary>
        /// Digital Object Identifier
        /// for each article
        /// </summary>
        [MaxLength(255)]
        public string DOI { get; set; }

        [NotMapped]
        public bool HasDOI => !string.IsNullOrEmpty(DOI);

        
        public bool IsSection { get; set; }

        public bool IsTheme { get; set; }


        [MaxLength(512)]
        public string Title { get; set; }

        [MaxLength(1024)]
        public string TitleNotes { get; set; }


        public int StartingPdfPage { get; set; }

        public int? StartingIndexPage { get; set; }

        public int LastPdfPage { get; set; }



        #region Navigation

        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public ApplicationLanguage Language { get; set; }


        [ForeignKey(nameof(Issue))]
        public int IssueId { get; set; }

        public ApplicationIssue Issue { get; set; }


        [ForeignKey(nameof(Pdf))]
        public int PdfId { get; set; }

        public ApplicationDocument Pdf { get; set; }


        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(127)]
        [ForeignKey(nameof(Author))]
        public string AuthorId { get; set; }

        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        public ApplicationUser Author { get; set; }

        [ForeignKey(nameof(Dossier))]
        public int? DossierId { get; set; }

        public ApplicationDossier Dossier { get; set; }


        public virtual ICollection<ApplicationAuthor> Authors { get; set; } = new List<ApplicationAuthor>();

        #endregion Navigation

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
