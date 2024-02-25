using linc.Models.Enumerations;

namespace linc.Data
{
    public class ApplicationDossier
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }


        public ApplicationDossierStatus Status { get; set; }


        public ICollection<ApplicationDocument> Documents { get; set; }
    }
}
