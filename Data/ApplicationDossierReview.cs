using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data
{
    public class ApplicationDossierReview
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }


        [NotMapped]
        public string Names => Reviewer == null ?
            $"{FirstName} {LastName}" :
            Reviewer.Names;


        #region Navigation

        [ForeignKey(nameof(Reviewer))]
        public string ReviewerId { get; set; }

        public ApplicationUser Reviewer { get; set; }

        [Required]
        [ForeignKey(nameof(Dossier))]
        public int DossierId { get; set; }

        public ApplicationDossier Dossier { get; set; }

        [Required]
        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; }

        public ApplicationDocument Review { get; set; }

        #endregion Navigation
    }
}
