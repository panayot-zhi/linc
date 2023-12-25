using linc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using linc.Utility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;

namespace linc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Codex()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                SiteCookieName.Language,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(CacheProfileName = "NoCache")]
        public IActionResult Error(string code)
        {
            if (Request.IsAjax())
            {
                if ("401".Equals(code))
                {
                    if (User.Identity is { IsAuthenticated: false })
                    {
                        return Challenge();
                    }
                }

                if (int.TryParse(code, out var candidate))
                {
                    return StatusCode(candidate);
                }

                _logger.LogWarning($"Processed an AJAX error with an unknown status code ({code}). Returning 500: Internal Server Error");

                return StatusCode(500);
            }

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (code)
            {
                case "400":
                {
                    return View("Error"); // return View("BadRequest");
                }
                case "401":
                {
                    if (User.Identity is { IsAuthenticated: true })
                    {
                        return Unauthorized();
                    }

                    return Challenge();
                }
                case "403":
                {
                    return View("Error"); // return View("Forbidden");
                }
                case "404":
                {
                    var path = "Could not resolve path.";
                    if (statusCodeReExecuteFeature != null)
                    {
                        path = statusCodeReExecuteFeature.GetFullPath();
                        ViewData["Path"] = path;
                    }

                    _logger.LogWarning("404 NotFound: " + path);

                    // TODO: List here any cases where we need response body brevity
                    if (path.EndsWith(".js.map"))
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, path);
                    }

                    var acceptHeaders = Request.Headers["Accept"];
                    if (acceptHeaders.Any(header => header.Contains("image/*")))
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, path);
                    }

                    return View("Error"); // return View("NotFound");
                }
            }

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (string.IsNullOrEmpty(code))
            {
                code = "500"; // internal server error
            }

            var errorViewModel = GetErrorViewModel(exceptionHandlerPathFeature, code);

            return View(errorViewModel);
        }

        protected ErrorViewModel GetErrorViewModel(IExceptionHandlerPathFeature? exceptionHandlerPathFeature, string code)
        {
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                StatusCode = code
            };

            if (statusCodeReExecuteFeature != null)
            {
                errorViewModel.Path = statusCodeReExecuteFeature.GetFullPath();
            }
            else if (exceptionHandlerPathFeature != null)
            {
                var error = exceptionHandlerPathFeature.Error;
                errorViewModel.Path = exceptionHandlerPathFeature.Path;
                errorViewModel.Error = error.GatherInternals();
                errorViewModel.ShortMessage = error.Message;
                errorViewModel.StackTrace = error.StackTrace;
            }

            return errorViewModel;
        }
    }
}