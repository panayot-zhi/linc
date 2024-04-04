using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Source
{
    public class SourceCreateViewModel
    {
        [MaxLength(512, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_Title", ResourceType = typeof(Resources.SharedResource))]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_TitleNotes", ResourceType = typeof(Resources.SharedResource))]
        public string TitleNotes { get; set; }



        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_FirstName", ResourceType = typeof(Resources.SharedResource))]
        public string FirstName { get; set; }

        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_LastName", ResourceType = typeof(Resources.SharedResource))]
        public string LastName { get; set; }

        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_AuthorNotes", ResourceType = typeof(Resources.SharedResource))]
        public string AuthorNotes { get; set; }



        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_StartingPage", ResourceType = typeof(Resources.SharedResource))]
        public int? StartingPage { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_LastPage", ResourceType = typeof(Resources.SharedResource))]
        public int? LastPage { get; set; }



        [Display(Name = "SourceCreate_PdfFile", ResourceType = typeof(Resources.SharedResource))]
        public IFormFile PdfFile { get; set; }



        [Display(Name = "SourceCreate_IssueId", ResourceType = typeof(Resources.SharedResource))]
        public int? IssueId { get; set; }

        [Display(Name = "SourceCreate_LanguageId", ResourceType = typeof(Resources.SharedResource))]
        public int LanguageId { get; set; }



        public List<SelectListItem> Languages { get; set; }

        public List<SelectListItem> Issues { get; set; }
    }

}
