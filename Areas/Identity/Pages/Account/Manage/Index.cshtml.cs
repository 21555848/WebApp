// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Areas.Identity.Data;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<WebAppUser> _userManager;
        private readonly SignInManager<WebAppUser> _signInManager;
        private readonly WebAppContext _context;

        public IndexModel(
            UserManager<WebAppUser> userManager,
            SignInManager<WebAppUser> signInManager,
            WebAppContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CellNo { get; set; }
            public string? AlternateCell { get; set; }
            //public string EmailAddress { get; set; }

            // public PatientAddress PatientAddress { get; set; }
            public string StreetAddress { get; set; }
            public string Address2 { get; set; }
            public string Suburb { get; set; }
            public string Province { get; set; }
            // public Work Work { get; set; }
            public string Company { get; set; }
            public string Occupation { get; set; }
            public string WorkPhone { get; set; }
            //Credit Card Information
           
            //Medical Aid information
            //public MedicalAid MedicalAid { get; set; }
            public string MedicalAidName { get; set; }
            public string MedicalAidNumber { get; set; }
            public DateOnly DoB { get; set; }
            public Gender Gender { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(WebAppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            var profile = _context.PatientProfile.FirstOrDefault(x => x.WebAppUserId == userId);

            var patientAddress = _context.PatientAddress.FirstOrDefault(x => x.PatientProfileId == profile.Id);
            var workInfo = _context.Work.FirstOrDefault(x => x.PatientProfileId == profile.Id);
            var medicalAidInfo = _context.MedicalAid.FirstOrDefault(x => x.PatientProfileId == profile.Id);

            var appUser = profile.WebAppUser;

            Username = userName;

            //if(profile.PatientAddress != null)
            //{
            //    Input = new InputModel
            //    {
            //        PhoneNumber = phoneNumber,
            //        FirstName = profile.FirstName,
            //        LastName = profile.LastName,
            //        CellNo = profile.CellNo,
            //        AlternateCell = profile.AlternateCell,
            //        StreetAddress = profile.PatientAddress.StreetAddress,
            //        Address2 = profile.PatientAddress.Address2,
            //        Suburb = profile.PatientAddress.Suburb,
            //        Province = profile.PatientAddress.Province,
            //        Company = profile.Work.Company,
            //        Occupation = profile.Work.Occupation,
            //        WorkPhone = profile.Work.WorkPhone,
            //        CardHolder = profile.CreditCard.CardHolder,
            //        CreditCardNo = profile.CreditCard.CreditCardNo,
            //        ExpiryDate = profile.CreditCard.ExpiryDate,
            //        CVV = profile.CreditCard.CVV,
            //        MedicalAidName = profile.MedicalAid.Name,
            //        MedicalAidNumber = profile.MedicalAid.Number
            //    };
            //}

            //else
            //{
            //    Input = new InputModel
            //    {
            //        PhoneNumber = phoneNumber,
            //        FirstName = profile.FirstName,
            //        LastName = profile.LastName,
            //        CellNo = profile.CellNo,
            //        AlternateCell = profile.AlternateCell
            //    };
            //}

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
               // FirstName = profile.FirstName,
               // LastName = profile.LastName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CellNo = profile.CellNo,
                AlternateCell = profile.AlternateCell,
                StreetAddress = patientAddress.StreetAddress,
                Address2 = patientAddress.Address2,
                Suburb = patientAddress.Suburb,
                Province = patientAddress.Province,
                Company = workInfo.Company,
                Occupation = workInfo.Occupation,
                WorkPhone = workInfo.WorkPhone,
                MedicalAidName = medicalAidInfo.Name,
                MedicalAidNumber = medicalAidInfo.Number
            };


        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var profile = _context.PatientProfile.FirstOrDefault(x => x.WebAppUserId == userId);

            var patientAddress = _context.PatientAddress.FirstOrDefault(x => x.PatientProfileId == profile.Id);
            var medicalAidInfo = _context.MedicalAid.FirstOrDefault(x => x.PatientProfileId == profile.Id);
            var workInfo = _context.Work.FirstOrDefault(x => x.PatientProfileId == profile.Id);

            if (profile == null)
            {
                return NotFound("Error with profile");
            }
           
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            profile.CellNo = Input.CellNo;
            profile.AlternateCell = Input.AlternateCell;
            patientAddress.StreetAddress = Input.StreetAddress;
            patientAddress.Address2 = Input.Address2;
            patientAddress.Suburb = Input.Suburb;
            patientAddress.Province = Input.Province;
            workInfo.Company = Input.Company;
            workInfo.Occupation = Input.Occupation;
            workInfo.WorkPhone = Input.WorkPhone;
            medicalAidInfo.Name = Input.MedicalAidName;
            medicalAidInfo.Number = Input.MedicalAidNumber;
            profile.DoB = Input.DoB.ToDateTime(new TimeOnly(0,0));
            profile.Gender = Input.Gender;

            _context.Update(profile);
            _context.Update(patientAddress);
            _context.Update(workInfo);
            _context.Update(medicalAidInfo);
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
