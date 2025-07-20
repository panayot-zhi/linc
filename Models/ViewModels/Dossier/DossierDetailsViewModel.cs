using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Author;

namespace linc.Models.ViewModels.Dossier
{
    public class DossierDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ApplicationDossierStatus Status { get; set; }

        public string AssignedToId { get; set; }

        [Display(Name = "DossierDetails_AssignedTo", ResourceType = typeof(Resources.SharedResource))]
        public string AssignedTo { get; set; }

        [Display(Name = "DateCreated_Label", ResourceType = typeof(Resources.SharedResource))]
        public DateTime DateCreated { get; set; }

        [Display(Name = "LastUpdated_Label", ResourceType = typeof(Resources.SharedResource))]
        public DateTime LastUpdated { get; set; }

        public List<ApplicationDossierReview> Reviews { get; set; }

        public List<ApplicationDocument> Documents { get; set; }

        public List<DossierJournal> Journals { get; set; }

        [Display(Name = "DossierDetails_Authors", ResourceType = typeof(Resources.SharedResource))]
        public List<DossierAuthorViewModel> Authors { get; set; } = new();
    }
}
