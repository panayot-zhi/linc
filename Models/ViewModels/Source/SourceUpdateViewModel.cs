using linc.Models.ViewModels.Author;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Source
{
    public class SourceUpdateViewModel : IValidatableObject
    {
        public int Id { get; set; }



        [MaxLength(512, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_Title", ResourceType = typeof(Resources.SharedResource))]
        public string Title { get; set; }

        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_TitleNotes", ResourceType = typeof(Resources.SharedResource))]
        public string TitleNotes { get; set; }



        [Display(Name = "SourceCreate_DOI", ResourceType = typeof(Resources.SharedResource))]
        public string DOI { get; set; }


        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceAuthor_FirstName", ResourceType = typeof(Resources.SharedResource))]
        public string FirstName { get; set; }

        [Obsolete("Do not use this property, it is pending deletion. Use the 'Authors' collection instead.")]
        [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceAuthor_LastName", ResourceType = typeof(Resources.SharedResource))]
        public string LastName { get; set; }

        [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_AuthorsNotes", ResourceType = typeof(Resources.SharedResource))]
        public string AuthorsNotes { get; set; }



        [Display(Name = "SourceCreate_StartingPdfPage", ResourceType = typeof(Resources.SharedResource))]
        public int? StartingPdfPage { get; set; }

        [Display(Name = "SourceCreate_LastPdfPage", ResourceType = typeof(Resources.SharedResource))]
        public int? LastPdfPage { get; set; }

        [Display(Name = "SourceCreate_StartingIndexPage", ResourceType = typeof(Resources.SharedResource))]
        public int? StartingIndexPage { get; set; }



        [Display(Name = "SourceCreate_PdfFile", ResourceType = typeof(Resources.SharedResource))]
        public IFormFile PdfFile { get; set; }



        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_IssueId", ResourceType = typeof(Resources.SharedResource))]
        public int IssueId { get; set; }

        [Display(Name = "SourceCreate_DossierId", ResourceType = typeof(Resources.SharedResource))]
        public int? DossierId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "SourceCreate_LanguageId", ResourceType = typeof(Resources.SharedResource))]
        public int LanguageId { get; set; }



        [Display(Name = "SourceCreate_IsTheme", ResourceType = typeof(Resources.SharedResource))]
        public bool IsTheme { get; set; }

        [Display(Name = "SourceCreate_IsSection", ResourceType = typeof(Resources.SharedResource))]
        public bool IsSection { get; set; }


        [Display(Name = "LastUpdated_Label", ResourceType = typeof(Resources.SharedResource))]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "DateCreated_Label", ResourceType = typeof(Resources.SharedResource))]
        public DateTime DateCreated { get; set; }


        [Display(Name = "SourceCreate_Authors", ResourceType = typeof(Resources.SharedResource))]
        public List<SourceAuthorViewModel> Authors { get; set; } = new();


        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Languages { get; set; }

        public List<SelectListItem> Issues { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            const int minimumAuthorsCount = 1;
            if (!IsSection && !IsTheme && (Authors == null || Authors.Count < minimumAuthorsCount))
            {
                yield return new ValidationResult(
                    string.Format(Resources.ValidationResource.MinCountAttribute_ValidationError,
                        Resources.SharedResource.SourceCreate_Authors, minimumAuthorsCount),
                    new[] { nameof(Authors) });
            }
        }
    }
}
