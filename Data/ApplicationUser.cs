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
        public bool DisplayEmail { get; set; }

        [PersonalData]
        public bool Subscribed { get; set; }

        [Obsolete("Author flag should be resolved through 'Authors' table with a relationship to 'Users'.")]
        [PersonalData]
        public bool IsAuthor { get; set; }

        [Obsolete("Reviewer flag should be resolved through 'Reviewers' table with a relationship to 'Users'.")]
        [PersonalData]
        public bool IsReviewer { get; set; }

        [PersonalData]
        public DateTime? LastLogin { get; set; }

        [PersonalData]
        [MaxLength(127)]
        public UserDisplayNameType DisplayNameType { get; set; }

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

        #endregion Avatar

        #region Navigation

        // TODO: figure out what to use preferred language for
        // - when sending emails

        [ForeignKey(nameof(PreferredLanguage))]
        public int PreferredLanguageId { get; set; } = SiteConstant.BulgarianCulture.Key;

        public ApplicationLanguage PreferredLanguage { get; set; }


        [InverseProperty(nameof(ApplicationUserProfile.User))]
        public ICollection<ApplicationUserProfile> Profiles { get; set; }

        [InverseProperty(nameof(ApplicationDossier.CreatedBy))]
        public ICollection<ApplicationDossier> CreatedDossiers { get; set; }

        [InverseProperty(nameof(ApplicationDossier.AssignedTo))]
        public ICollection<ApplicationDossier> AssignedDossiers { get; set; }

        #endregion Navigation

        #region NotMapped

        [NotMapped]
        public ApplicationUserProfile CurrentProfile => Profiles.First(x => x.LanguageId == PreferredLanguageId);
        
        [NotMapped]
        [MaxLength(1024)]
        public string Description => CurrentProfile.Description;
        
        [ProtectedPersonalData]
        public string FirstName => CurrentProfile.FirstName;
        
        [Required]
        [MaxLength(255)]
        [ProtectedPersonalData]
        public string LastName => CurrentProfile.LastName;
        
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
