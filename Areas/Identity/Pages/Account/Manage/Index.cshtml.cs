﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using linc.Contracts;
using linc.Data;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace linc.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : BasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILocalizationService localizationService, 
            ILogger<IndexModel> logger)
        : base(localizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "RegisterModel_FirstName", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string FirstName { get; set; }

            [Display(Name = "RegisterModel_LastName", ResourceType = typeof(Resources.SharedResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [MaxLength(255, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string LastName { get; set; }

            [Display(Name = "RegisterModel_UserName", ResourceType = typeof(Resources.SharedResource))]
            [MaxLength(127, ErrorMessageResourceName = "MaxLengthAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            [Required(ErrorMessageResourceName = "RequiredAttribute_ValidationError", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string UserName { get; set; }

            [Display(Name = "ManageProfile_Phone", ResourceType = typeof(Resources.SharedResource))]
            [Phone(ErrorMessageResourceName = "PhoneAttribute_Invalid", ErrorMessageResourceType = typeof(Resources.ValidationResource))]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,

                UserName = userName,
                PhoneNumber = phoneNumber
            };
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

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.Errors.Any())
                {
                    ModelState.AddIdentityErrors(result.Errors);
                    return Page();
                }

                StatusMessage = ErrorStatusMessage(
                    LocalizationService["ManageIndex_SetNames_ErrorStatusMessage"]
                );
                return RedirectToPage();
            }

            var username = await _userManager.GetUserNameAsync(user);
            if (Input.UserName != username)
            {
                result = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!result.Succeeded)
                {
                    if (result.Errors.Any())
                    {
                        ModelState.AddIdentityErrors(result.Errors);
                        return Page();
                    }

                    StatusMessage = ErrorStatusMessage(
                        LocalizationService["ManageIndex_SetUserName_ErrorStatusMessage"]
                    );
                    return RedirectToPage();
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                result = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!result.Succeeded)
                {
                    if (result.Errors.Any())
                    {
                        ModelState.AddIdentityErrors(result.Errors);
                        return Page();
                    }

                    StatusMessage = ErrorStatusMessage(
                        LocalizationService["ManageIndex_SetPhoneNumber_ErrorStatusMessage"]
                    );
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = SuccessStatusMessage(
                LocalizationService["ManageIndex_SuccessStatusMessage"]
            );
            return RedirectToPage();
        }
    }
}
