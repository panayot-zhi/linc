using linc.Models.Enumerations;
using linc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace linc.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index(IndexCategory id = IndexCategory.All)
        {
            var viewModel = new LibraryViewModel()
            {
                Category = id
            };

            return View(viewModel);
        }
    }
}
