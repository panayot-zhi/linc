using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using linc.Models.Enumerations;
using linc.Utility;

namespace linc.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [MaxLength(1024)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        [ProtectedPersonalData]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        [ProtectedPersonalData]
        public string LastName { get; set; }

        [PersonalData]
        [MaxLength(127)]
        public UserDisplayNameType DisplayNameType { get; set; }

        [PersonalData]
        public bool DisplayEmail { get; set; }

        [PersonalData]
        public bool Subscribed { get; set; }

        public bool IsAuthor { get; set; }

        public bool IsReviewer { get; set; }


        #region Avatar

        [PersonalData]
        public UserAvatarType AvatarType { get; set; }

        [PersonalData]
        public string FacebookAvatarPath { get; set; }

        [PersonalData]
        public string TwitterAvatarPath { get; set; }

        [PersonalData]
        public string GoogleAvatarPath { get; set; }

        [PersonalData]
        public string InternalAvatarPath { get; set; }

        [PersonalData]
        public DateTime? LastLogin { get; set; }

        #endregion Avatar

        #region Navigation

        [InverseProperty(nameof(ApplicationDossier.CreatedBy))]
        public ICollection<ApplicationDossier> CreatedDossiers { get; set; }

        [InverseProperty(nameof(ApplicationDossier.AssignedTo))]
        public ICollection<ApplicationDossier> AssignedDossiers { get; set; }

        #endregion Navigation

        #region NotMapped

        [NotMapped]
        [PersonalData]
        public string Names => $"{FirstName} {LastName}";

        #endregion NotMapped

        #region Automatic

        [PersonalData]
        public DateTime LastUpdated { get; set; }

        [PersonalData]
        public DateTime DateCreated { get; set; }

        #endregion Automatic
    }

}
