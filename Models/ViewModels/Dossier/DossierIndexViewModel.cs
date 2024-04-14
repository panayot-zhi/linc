using linc.Data;
using linc.Utility;

namespace linc.Models.ViewModels.Dossier
{
    public class DossierIndexViewModel : PagedViewModel
    {
        public DossierIndexViewModel(int totalRecords, int pageIndex, int pageSize) : base(totalRecords, pageIndex, pageSize)
        {
        }

        public IEnumerable<ApplicationDossier> Records { get; set; }
    }
}
