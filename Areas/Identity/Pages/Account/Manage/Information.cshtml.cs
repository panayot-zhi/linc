using System.ComponentModel.DataAnnotations;
using linc.Contracts;
using linc.Data;
using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace linc.Areas.Identity.Pages.Account.Manage
{
    public partial class InfoModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<InfoModel> _logger;

        public InfoModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILocalizationService localizationService,
            ILogger<InfoModel> logger)
        : base(localizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string CurrentDisplayName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "ManagePreferences_DisplayNameType", ResourceType = typeof(Resources.SharedResource))]
            public UserDisplayNameType DisplayNameType { get; set; }

            [Display(Name = "ManagePreferences_DisplayEmail", ResourceType = typeof(Resources.SharedResource))]
            public bool DisplayEmail { get; set; }

            [Display(Name = "ManagePreferences_Description", ResourceType = typeof(Resources.SharedResource))]
            [MaxLength(1024, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string Description { get; set; }

            [Display(Name = "ManagePreferences_Subscribed", ResourceType = typeof(Resources.SharedResource))]
            public bool Subscribed { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            Input = new InputModel
            {
                DisplayEmail = user.DisplayEmail,
                DisplayNameType = user.DisplayNameType,
                Description = user.Description,
                Subscribed = user.Subscribed
            };

            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            CurrentDisplayName = user.GetDisplayName(LocalizationService);

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to find user with ID {UserId}",
                    _userManager.GetUserId(User));

                return NotFound();
            }

            await LoadAsync(user);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to find user with ID {UserId}",
                    _userManager.GetUserId(User));
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.DisplayEmail = Input.DisplayEmail;
            user.DisplayNameType = Input.DisplayNameType;
            user.Description = Input.Description;
            user.Subscribed = Input.Subscribed;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.Errors.Any())
                {
                    ModelState.AddIdentityErrors(result.Errors);
                    return Page();
                }

                StatusMessage = ErrorStatusMessage(
                    LocalizationService["ManagePreferences_Update_ErrorStatusMessage"]
                );
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = SuccessStatusMessage(
                LocalizationService["ManagePreferences_Update_SuccessStatusMessage"]
            );
            return RedirectToPage();
        }
    }
}
