using linc.Data;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace linc.Models.ViewModels.Dossier
{
    public class DossierDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ApplicationDossierStatus Status { get; set; }

        public string AuthorNames { get; set; }

        public string AuthorEmail { get; set; }

        public string AssignedToId { get; set; }

        public string AssignedTo { get; set; }


        public List<ApplicationDocument> Documents { get; set; }

        public List<DossierJournal> Journals { get; set; }

    }
}
