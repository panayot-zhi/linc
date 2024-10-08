﻿using System.ComponentModel.DataAnnotations;
using linc.Contracts;
using linc.Data;
using linc.Models.Enumerations;
using linc.Services;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace linc.Areas.Identity.Pages.Account.Manage
{
    public partial class InfoModel : BasePageModel
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<InfoModel> _logger;

        public InfoModel(
            ApplicationUserManager userManager,
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

        public List<SelectListItem> SupportedLanguages { get; set; }

        public class InputModel
        {
            public ApplicationUserProfile[] Profiles { get; set; }


            [Display(Name = nameof(Resources.SharedResource.ManagePreferences_PreferredLanguageId), ResourceType = typeof(Resources.SharedResource))]
            public int PreferredLanguageId { get; set; }

            [Display(Name = "ManagePreferences_DisplayNameType", ResourceType = typeof(Resources.SharedResource))]
            public UserDisplayNameType DisplayNameType { get; set; }

            [Display(Name = "ManagePreferences_DisplayEmail", ResourceType = typeof(Resources.SharedResource))]
            public bool DisplayEmail { get; set; }

            [Display(Name = "ManagePreferences_Subscribed", ResourceType = typeof(Resources.SharedResource))]
            public bool Subscribed { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            Input = new InputModel
            {
                Profiles = user.Profiles.ToArray(),

                PreferredLanguageId = user.PreferredLanguageId,
                DisplayEmail = user.DisplayEmail,
                DisplayNameType = user.DisplayNameType,
                Subscribed = user.Subscribed
            };

            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            CurrentDisplayName = user.GetDisplayName(LocalizationService);
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            SupportedLanguages = SiteConstant.SupportedCultures.Select(x => new SelectListItem()
            {
                Value = x.Key.ToString(),
                Text = x.Value,

                Selected = x.Key == user.PreferredLanguageId

            }).ToList();
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
            user.Subscribed = Input.Subscribed;

            if (Input.PreferredLanguageId != user.PreferredLanguageId)
            {
                user.PreferredLanguageId = Input.PreferredLanguageId;
                Response.SetCurrentLanguage(user.PreferredLanguageId);
            }

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

            foreach (var profile in Input.Profiles)
            {
                var userProfile = user.Profiles.First(x =>
                    x.LanguageId == profile.LanguageId);

                if (userProfile.Description != profile.Description)
                    userProfile.Description = profile.Description;
            }

            result = await _userManager.UpdateUserProfiles(user);

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
