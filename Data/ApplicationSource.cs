using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace linc.Data
{
    [Index(nameof(FirstName), IsUnique = false)]
    [Index(nameof(LastName), IsUnique = false)]
    public class ApplicationSource
    {
        public int Id { get; set; }


        [MaxLength(255)]
        public string FirstName { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; }

        [NotMapped] public string AuthorNames => $"{FirstName} {LastName}";

        [MaxLength(1024)]
        public string AuthorNotes { get; set; }


        [MaxLength(512)]
        public string Title { get; set; }

        [MaxLength(1024)]
        public string TitleNotes { get; set; }


        public int StartingPage { get; set; }

        public int LastPage { get; set; }


        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public ApplicationLanguage Language { get; set; }


        [ForeignKey(nameof(Issue))]
        public int? IssueId { get; set; }

        public ApplicationIssue Issue { get; set; }


        [ForeignKey(nameof(Pdf))]
        public int? PdfId { get; set; }

        public ApplicationDocument Pdf { get; set; }


        [MaxLength(127)]
        [ForeignKey(nameof(Author))]
        public string AuthorId { get; set; }

        public ApplicationUser Author { get; set; }


        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}
