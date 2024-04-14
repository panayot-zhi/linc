using linc.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using linc.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
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

        public DossierEditViewModel()
        {

        }


        public int Id { get; set; }


        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }


        public ApplicationDossierStatus Status { get; set; }


        public string AssigneeId { get; set; }

        public string Assignee { get; set; }


        public List<SelectListItem> Editors { get; set; }


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
