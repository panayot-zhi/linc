using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data
{
    [Index(nameof(Names), IsUnique = false)]
    [Index(nameof(Names), nameof(SourceId), IsUnique = true)]
    [Index(nameof(Names), nameof(DossierId), IsUnique = true)]
    [Index(nameof(Names), nameof(SourceId), nameof(DossierId), IsUnique = true)]
    [Index(nameof(FirstName), IsUnique = false)]
    [Index(nameof(LastName), IsUnique = false)]
    [Index(nameof(Email), IsUnique = false)]
    public class ApplicationAuthor
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(1024)]
        public string Notes { get; set; }

        // NOTE: Computed
        [MaxLength(512)]
        public string Names { get; set; }



        #region Navigation

        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public ApplicationLanguage Language { get; set; }

        [MaxLength(127)]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(Source))]
        public int? SourceId { get; set; }

        public ApplicationSource Source { get; set; }

        [ForeignKey(nameof(Dossier))]
        public int? DossierId { get; set; }

        public ApplicationDossier Dossier { get; set; }

        [ForeignKey(nameof(Agreement))]
        public int? AgreementId { get; set; }

        public ApplicationDocument Agreement { get; set; }

        #endregion Navigation

        #region Automatic

        public DateTime LastUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion
    }
}