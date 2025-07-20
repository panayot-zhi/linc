using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Author;
using Microsoft.AspNetCore.Mvc.Rendering;
using linc.Contracts;

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

        // New: Authors collection for Dossier
        [Required]
        [MinLength(1, ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
        public List<DossierAuthorViewModel> Authors { get; set; } = new();


        public ApplicationDossierStatus Status { get; set; }

        public bool SuperReviewed { get; set; }


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

        [Display(Name = "DocumentType_Agreement", ResourceType = typeof(Resources.SharedResource))]
        public ApplicationDocument Agreement =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Agreement);

        public ApplicationDocument Redacted =>
            _documents.FirstOrDefault(x => x.DocumentType == ApplicationDocumentType.Redacted);

        public List<ApplicationDocument> Reviews =>
            _documents.Where(x => x.DocumentType is ApplicationDocumentType.Review or ApplicationDocumentType.SuperReview).ToList();


        [Display(Name = "DossierEdit_Agreement", ResourceType = typeof(Resources.SharedResource))]
        public IFormFile AgreementDocument { get; set; }


        public ApplicationDocumentType DocumentType { get; set; }

        public IFormFile Document { get; set; }


        public bool CanAttachAgreement { get; init; }

        public bool CanDeleteAgreement { get; init; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status == ApplicationDossierStatus.InReview && Document != null)
            {
                var localizationService = validationContext.GetService(typeof(ILocalizationService)) as ILocalizationService;

                ArgumentNullException.ThrowIfNull(localizationService);

                if (string.IsNullOrEmpty(ReviewerFirstName))
                {
                    yield return new ValidationResult(localizationService["DossierEdit_ReviewerFirstName_Required"].Value, 
                        new[] { nameof(ReviewerFirstName) });
                }

                if (string.IsNullOrEmpty(ReviewerLastName))
                {
                    yield return new ValidationResult(localizationService["DossierEdit_ReviewerLastName_Required"].Value,
                        new[] { nameof(ReviewerLastName) });
                }

                if (string.IsNullOrEmpty(ReviewerEmail))
                {
                    yield return new ValidationResult(localizationService["DossierEdit_ReviewerEmail_Required"].Value,
                        new[] { nameof(ReviewerEmail) });
                }
            }

            yield break;
        }
    }
}
