using linc.Models.ViewModels.Author;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
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
        [Display(Name = "DossierCreate_OriginalFile", ResourceType = typeof(Resources.SharedResource))]
        public IFormFile OriginalFile { get; set; }

        // New: Authors collection for Dossier
        [Required]
        [MinLength(1, ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public List<DossierAuthorViewModel> Authors { get; set; } = new();

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_LanguageId", ResourceType = typeof(Resources.SharedResource))]
        public int LanguageId { get; set; }

        public List<SelectListItem> Languages { get; set; }
    }
}
