using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Author
{
    public class SourceAuthorViewModel
    {
        public int? Id { get; set; }

        public int? DossierId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "Author_FirstName", ResourceType = typeof(Resources.SharedResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "Author_LastName", ResourceType = typeof(Resources.SharedResource))]
        public string LastName { get; set; }

        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "Author_Notes", ResourceType = typeof(Resources.SharedResource))]
        public string Notes { get; set; }

        public string Names { get; set; }


        [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "Author_Email", ResourceType = typeof(Resources.SharedResource))]
        public string Email { get; set; }


        [MaxLength(127)]
        public string UserId { get; set; }

        [Display(Name = "Author_UserName", ResourceType = typeof(Resources.SharedResource))]
        public string UserName { get; set; }
    }
}
