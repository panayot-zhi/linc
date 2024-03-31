using System.Diagnostics;
using System.Net;
using linc.Data;
using linc.Utility;
using linc.Contracts;
using linc.Models.Enumerations;
using linc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using linc.Models.ConfigModels;
using linc.Models.ViewModels.Emails;
using Newtonsoft.Json;
using System.Text;

namespace linc.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContentService _contentService;
        private readonly ISiteEmailSender _mailSender;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ILocalizationService localizationService, 
            IContentService contentService, 
            ISiteEmailSender mailSender)
        : base(localizationService)
        {
            _logger = logger;
            _contentService = contentService;
            _mailSender = mailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _contentService.GetIndexViewModel();
            return View(viewModel);
        }

        [SiteAuthorize(SiteRole.Editor)]
        public IActionResult Edit()
        {
            if (TempData.ContainsKey(SiteConstant.TempDataEditableKey))
            {
                TempData.Remove(SiteConstant.TempDataEditableKey);
            }
            else
            {
                TempData.Add(SiteConstant.TempDataEditableKey, SiteConstant.True);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Submit()
        {
            throw new NotImplementedException();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Cookies()
        {
            return View();
        }

        [Ajax]
        [HttpPost]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> SetStringResource([FromBody][Bind("Key,Value,EditedById")] ApplicationStringResource stringResource)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState
                    .Where(modelState => modelState.Value?.Errors.Count > 0)
                    .Select(modelState => 
                        modelState.Key + ": " + string.Join(",", modelState.Value.Errors.Select(error => error.ErrorMessage)))
                    .ToList();

                _logger.LogError("Request from {UserId} contained invalid information: {@ModelStateErrors}",
                    userId, modelStateErrors);

                return BadRequest();
            }

            await LocalizationService.SetStringResource(stringResource.Key, stringResource.Value, userId);

            AddAlertMessage(LocalizationService["SetStringResource_Success"],
                type: AlertMessageType.Success);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                SiteCookieName.Language,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(7)
                }
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(CacheProfileName = "NoCache")]
        public IActionResult Error(string id)
        {
            var errorViewModel = GetErrorViewModel(id);

            if (Request.IsAjax())
            {
                if ("401".Equals(id))
                {
                    if (User.Identity is { IsAuthenticated: false })
                    {
                        return Challenge();
                    }
                }

                if (int.TryParse(id, out var candidate))
                {
                    return StatusCode(candidate);
                }

                _logger.LogWarning("Processed an AJAX error with an unknown status code ({StatusCode}) for request ({RequestId}) bound for path {Path}. Returning 500: Internal Server Error",
                    errorViewModel.StatusCode, errorViewModel.RequestId, errorViewModel.Path);

                return StatusCode(500);
            }

            switch (id)
            {
                case "400":
                {
                    _logger.LogWarning("400 BadRequest ({RequestId}): {Path}",
                        errorViewModel.RequestId, errorViewModel.Path);

                    return View("BadRequest", errorViewModel);
                }
                case "401":
                {
                    if (User.Identity is { IsAuthenticated: true })
                    {
                        _logger.LogWarning("401 Unauthorized ({RequestId}): {Path}",
                            errorViewModel.RequestId, errorViewModel.Path);

                        return Unauthorized();
                    }

                    return Challenge();
                }
                case "403":
                {
                    _logger.LogWarning("403 Forbidden ({RequestId}): {Path}",
                        errorViewModel.RequestId, errorViewModel.Path);

                    return View("Forbidden", errorViewModel);
                }
                case "404":
                {
                    var path = errorViewModel.Path;

                    _logger.LogWarning("404 NotFound ({RequestId}): {Path}",
                        errorViewModel.RequestId, errorViewModel.Path);

                    // NOTE: List here any cases where
                    // we need response body brevity
                    
                    if (path.EndsWith(".js.map"))
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, path);
                    }

                    var acceptHeaders = Request.Headers["Accept"];
                    if (acceptHeaders.Any(header => header.Contains("image/*")))
                    {
                        return StatusCode((int)HttpStatusCode.NotFound, path);
                    }

                    return View("NotFound", errorViewModel);
                }
            }

            // NOTE: try to get some more exception details from the request

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (string.IsNullOrEmpty(id))
            {
                id = "500";
            }

            errorViewModel = GetErrorViewModel(id, exceptionHandlerPathFeature);

            _logger.LogError("500 InternalServerError ({RequestId}) at {Path}: {ErrorMessage}",
                errorViewModel.RequestId, errorViewModel.Path, errorViewModel.Error);

            return View(errorViewModel);
        }

        [SiteAuthorize(SiteRole.Administrator, andAbove: false)]
        public async Task<IActionResult> TestSendEmail(string id)
        {
            var request = HttpContext.Request;
            var domainUrl = $"{request.Scheme}://{request.Host}";
            
            var viewModel = new TestEmail()
            {
                Test = LocalizationService["Logo_Long"].Value,
                TestButton = new EmailButton
                {
                    Text = LocalizationService["Logo_Short"].Value,
                    Url = domainUrl
                }
            };

            if (!string.IsNullOrEmpty(id))
            {
                // test language display
                viewModel.Language = new CultureInfo(id).Name;
            }

            var email = new SiteEmailDescriptor<TestEmail>()
            {
                Emails = new()
                {
                    SiteConstant.AdministratorEmail
                },
                Subject = "Коле, получи ли?",
                ViewModel = viewModel
            };

            await _mailSender.SendEmailAsync(email);

            //AddAlertMessage("Sent");

            return View($"Emails/TestEmail.{viewModel.Language}", viewModel);
        }

        protected ErrorViewModel GetErrorViewModel(string code, IExceptionHandlerPathFeature exceptionHandlerPathFeature = null)
        {
            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                StatusCode = code
            };

            if (Activity.Current is { Id: { } })
            {
                errorViewModel.RequestId = Activity.Current.Id;
            }

            if (!string.IsNullOrEmpty(Request.Query["ReturnUrl"]))
            {
                errorViewModel.Path = Request.Query["ReturnUrl"];
            }

            if (statusCodeReExecuteFeature != null)
            {
                errorViewModel.Path = statusCodeReExecuteFeature.GetFullPath();
            }

            if (exceptionHandlerPathFeature != null)
            {
                var error = exceptionHandlerPathFeature.Error;
                errorViewModel.Path = exceptionHandlerPathFeature.Path;
                errorViewModel.Error = error.GatherInternals();
                errorViewModel.ShortMessage = error.Message;
                errorViewModel.StackTrace = error.StackTrace;
            } 
            

            return errorViewModel;
        }

        public IActionResult Email(string id, string data)
        {
            var type = Type.GetType($"linc.Models.ViewModels.Emails.{id}");
            
            ArgumentNullException.ThrowIfNull(type);

            var base64EncodedBytes = Convert.FromBase64String(data);
            var jsonViewModel = Encoding.UTF8.GetString(base64EncodedBytes);
            var viewModel = JsonConvert.DeserializeObject(jsonViewModel, type);

            if (viewModel is not BaseEmailViewModel baseViewModel)
            {
                throw new NotSupportedException("Message could not be read.");
            }
                
            baseViewModel.Preview = "#";
            baseViewModel.IsPreviewing = true;

            return View($"~/Views/Shared/Emails/{id}.{baseViewModel.Language}.cshtml", viewModel);
        }
    }
}