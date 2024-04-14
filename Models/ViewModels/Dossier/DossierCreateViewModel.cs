using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Dossier
{
    public class DossierCreateViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_Title", ResourceType = typeof(Resources.SharedResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_FirstName", ResourceType = typeof(Resources.SharedResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_LastName", ResourceType = typeof(Resources.SharedResource))]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_Email", ResourceType = typeof(Resources.SharedResource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_OriginalFile", ResourceType = typeof(Resources.SharedResource))]
        public IFormFile OriginalFile { get; set; }
    }
}
