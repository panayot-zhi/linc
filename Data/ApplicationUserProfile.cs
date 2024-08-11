using linc.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace linc.Data
{
    public class ApplicationUserProfile
    {
        [PersonalData]
        [MaxLength(1024)]
        public string Description { get; set; }

        [MaxLength(255)]
        [ProtectedPersonalData]
        public string FirstName { get; set; }

        [MaxLength(255)]
        [ProtectedPersonalData]
        public string LastName { get; set; }


        #region Navigation

        [ForeignKey(nameof(Language))]
        public int LanguageId { get; set; }

        public ApplicationLanguage Language { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        #endregion Navigation


        #region NotMapped

        [NotMapped]
        [PersonalData]
        public string Names => $"{FirstName} {LastName}";

        #endregion NotMapped
    }
}
