using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;

namespace WebApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the WebAppUser class
public class WebAppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public PatientProfile? PatientProfile { get; set; }
    public Doctor? Doctor { get; set; }
    public DateTime? LastLogon { get; set; }
}

