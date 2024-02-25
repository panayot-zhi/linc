using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Issue
{
    public class IssueCreateViewModel
    {
        [Display(Name = "IssueCreate_IssueNumber", ResourceType = typeof(Resources.SharedResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public int? IssueNumber { get; set; }

        [Display(Name = "IssueCreate_ReleaseYear", ResourceType = typeof(Resources.SharedResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public int? ReleaseYear { get; set; }

        [Display(Name = "IssueCreate_Description", ResourceType = typeof(Resources.SharedResource))]
        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_InvalidMaxLength", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public string Description { get; set; }

        [Display(Name = "IssueCreate_PdfFile", ResourceType = typeof(Resources.SharedResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public IFormFile PdfFile { get; set; }

        [Display(Name = "IssueCreate_CoverPage", ResourceType = typeof(Resources.SharedResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public IFormFile CoverPage { get; set; }

        [Display(Name = "IssueCreate_IndexPages", ResourceType = typeof(Resources.SharedResource))]
        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public Collection<IFormFile> IndexPages { get; set; }
    }
}
