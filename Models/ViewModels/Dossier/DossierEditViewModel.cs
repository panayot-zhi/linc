using linc.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using linc.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Dossier
{
    public class DossierEditViewModel : IValidatableObject
    {
        private readonly List<ApplicationDocument> _documents = new();

        public DossierEditViewModel(List<ApplicationDocument> documents)
        {
            _documents = documents;
        }

        // ReSharper disable once UnusedMember.Global
        // NOTE: this is done for model binding
        public DossierEditViewModel()
        {

        }


        public int Id { get; set; }


        [Display(Name = "DossierCreate_Title", ResourceType = typeof(Resources.SharedResource))]
        public string Title { get; set; }

        [Display(Name = "DossierCreate_FirstName", ResourceType = typeof(Resources.SharedResource))]
        public string FirstName { get; set; }

        [Display(Name = "DossierCreate_LastName", ResourceType = typeof(Resources.SharedResource))]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierCreate_Email", ResourceType = typeof(Resources.SharedResource))]
        public string Email { get; set; }


        public ApplicationDossierStatus Status { get; set; }


        [Display(Name = "DossierEdit_Assignee", ResourceType = typeof(Resources.SharedResource))]
        public string AssigneeId { get; set; }

        [Display(Name = "DossierEdit_Assignee", ResourceType = typeof(Resources.SharedResource))]
        public string AssigneeNames { get; set; }


        [Display(Name = "DossierEdit_ReviewerFirstName", ResourceType = typeof(Resources.SharedResource))]
        public string ReviewerFirstName { get; set; }

        [Display(Name = "DossierEdit_ReviewerLastName", ResourceType = typeof(Resources.SharedResource))]
        public string ReviewerLastName { get; set; }

        [EmailAddress(ErrorMessageResourceName = "EmailAddressAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        [Display(Name = "DossierEdit_ReviewerEmail", ResourceType = typeof(Resources.SharedResource))]
        public string ReviewerEmail { get; set; }


        [Display(Name = "DossierEdit_Editors", ResourceType = typeof(Resources.SharedResource))]
        public List<SelectListItem> Editors { get; set; }

        [Display(Name = "DossierEdit_Reviewers", ResourceType = typeof(Resources.SharedResource))]
        public List<SelectListItem> Reviewers { get; set; }


        public ApplicationDocument Original =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Original);

        public ApplicationDocument Anonymized =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Anonymized);

        public ApplicationDocument Agreement =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Agreement);

        public ApplicationDocument Redacted =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Redacted);

        public List<ApplicationDocument> Reviews =>
            _documents.Where(x => x.DocumentType == ApplicationDocumentType.Review).ToList();


        public ApplicationDocumentType DocumentType { get; set; }

        public IFormFile Document { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //var localizationService = validationContext.GetService(typeof(ILocalizationService)) as ILocalizationService;

            // TODO: Validate!
            //yield return new ValidationResult("Моля факайте се.", new[] { nameof(Id) });

            yield break;
        }
    }
}
