using linc.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

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
    }
}
