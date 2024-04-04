using linc.Contracts;

namespace linc.Controllers
{
    public class DocumentController : BaseController
    {
        public DocumentController(
            ILocalizationService localizationService) 
            : base(localizationService)
        {
        }

    }
}
