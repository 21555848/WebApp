using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Identity.Data;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class InternalController : Controller
    {
        private readonly WebAppContext _context;
        private readonly UserManager<WebAppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private List<WebAppUser> _systemUsers;

        public InternalController(WebAppContext context, RoleManager<IdentityRole> roleManager,UserManager<WebAppUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
          //  _systemUsers = _userManager.Users.ToList();
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            // List<PatientListViewModel> patients = new List<PatientListViewModel>();
            // var webAppContext = _userManager.Users.Where(p => p.PatientProfile != null).ToList();
            var profiles = _context.PatientProfile.Include(x => x.WebAppUser);
            
            return View(await profiles.ToListAsync());
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doctor == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Suite)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        [Authorize(Roles ="SuperUser,Admin")]
        public IActionResult AddDoctor()
        {
            var unAssignedSuites = _context.Suite.Where(d => d.Doctor == null);
            ViewData["SuiteId"] = new SelectList(unAssignedSuites, "Id", "Name");
            ViewData["Count"] = unAssignedSuites.ToList().Count;
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor([Bind("FirstName,LastName,SuiteId,EmailAddress")] DoctorMaintenanceModel doctor)
        {
            if (ModelState.IsValid)
            {
                Doctor doc = new Doctor
                {
                    // FirstName = doctor.FirstName,
                    //LastName = doctor.LastName,
                    SuiteId = doctor.SuiteId,
                    Active = true
                    
                };

                WebAppUser doctorUser = new WebAppUser
                {
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.EmailAddress,
                    UserName = doctor.EmailAddress,
                    EmailConfirmed = true
                };

                var user = await _userManager.FindByEmailAsync(doctorUser.Email);
               // var user = _systemUsers.FirstOrDefault(a => a.Email == doctorUser.Email);
                if(user == null)
                {
                    await _userManager.CreateAsync(doctorUser, "Welcome1@");
                    await _userManager.AddToRoleAsync(doctorUser, "Doctor");
                }
                else
                {
                    ViewData["SuiteId"] = new SelectList(_context.Suite, "Id", "Name", doctor.SuiteId);
                    return View(doctor);
                }
                var forUserId = _userManager.FindByEmailAsync(doctor.EmailAddress).Result;
                doc.WebAppUserId = forUserId.Id;
                _context.Add(doc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Doctors));
            }
            ViewData["SuiteId"] = new SelectList(_context.Suite, "Id", "Name", doctor.SuiteId);
            return View(doctor);
        }

        public IActionResult Doctors()
        {
            List<DoctorMaintenanceModel> doctors = new List<DoctorMaintenanceModel>();
            var docs = _context.Doctor.ToList();
            foreach (var doc in docs)
            {
                var user = _userManager.FindByIdAsync(doc.WebAppUserId).Result;
                var suite = _context.Suite.FirstOrDefault(x => x.Id == doc.SuiteId);
                var model = new DoctorMaintenanceModel
                {
                    DoctorId = doc.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.Email,
                    SuiteName = suite.Name,
                    Active = doc.Active
                };
                doctors.Add(model);
            }

            return View(doctors);
        }
        public IActionResult Administrators()
        {
            return View(GetAdministrators());
        }

        [Authorize(Roles ="SuperUser")]
        public IActionResult AddAdministrator()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdministrator([Bind("FirstName,LastName,EmailAddress")]AdministratorModel administrator)
        {
            if (ModelState.IsValid)
            {
                WebAppUser adminUser = new WebAppUser
                {
                    FirstName = administrator.FirstName,
                    LastName = administrator.LastName,
                    Email = administrator.EmailAddress,
                    UserName = GenerateUsername(administrator.FirstName, administrator.LastName),
                    EmailConfirmed = true
                };

                var user = await _userManager.FindByEmailAsync(adminUser.Email);
                if(user == null)
                {
                    await _userManager.CreateAsync(adminUser, "Welcome1@");
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    EmailConfig email = new EmailConfig();
                    string body = string.Empty;
                    using(var reader = new StreamReader(Path.GetFullPath("EmailTemplates/NewUser.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{User}", adminUser.FirstName);
                    body = body.Replace("{UserType}", "Admin");
                    body = body.Replace("{Username}", adminUser.UserName);
                    body = body.Replace("{Password}", "Welcome1@");
                   
                    email.SendEmail(adminUser.Email, "New Admin User Credentials", body);   

                    return RedirectToAction(nameof(SystemUsers));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User with " + adminUser.Email + " already exists.");
                    return View(administrator);
                }
            }
            ModelState.AddModelError(string.Empty, "Error adding user, please verify your input.");
            return View(administrator);
        }

        



        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doctor == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SuiteId"] = new SelectList(_context.Suite, "Id", "Name", doctor.SuiteId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,SuiteId")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SuiteId"] = new SelectList(_context.Suite, "Id", "Id", doctor.SuiteId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> DeactivateDoctor(int? id)
        {
            if (id == null || _context.Doctor == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Suite)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (doctor == null)
            {
                return NotFound();
            }
            var userProfile = _userManager.FindByIdAsync(doctor.WebAppUserId).Result;
            DoctorMaintenanceModel doc = new DoctorMaintenanceModel
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                EmailAddress = userProfile.Email,
                SuiteName = doctor.Suite.Name,
                Active = doctor.Active
            };

            return View(doc);
        }

        // POST: Doctors/DeactivateDoctor/5
        [HttpPost, ActionName("DeactivateDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            if (_context.Doctor == null)
            {
                return Problem("Entity set 'WebAppContext.Doctor'  is null.");
            }
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor != null)
            {
                doctor.Active = false;
                _context.Update(doctor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Doctors));
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> ActivateDoctor(int? id)
        {
            if (id == null || _context.Doctor == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Suite)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }
            var userProfile = _userManager.FindByIdAsync(doctor.WebAppUserId).Result;
            DoctorMaintenanceModel doc = new DoctorMaintenanceModel
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                EmailAddress = userProfile.Email,
                SuiteName = doctor.Suite.Name,
                Active = doctor.Active
            };

            return View(doc);
        }

        // POST: Doctors/DeactivateDoctor/5
        [HttpPost, ActionName("ActivateDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            if (_context.Doctor == null)
            {
                return Problem("Entity set 'WebAppContext.Doctor'  is null.");
            }
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor != null)
            {
                doctor.Active = true;
                _context.Update(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Doctors));
        }

        private bool DoctorExists(int id)
        {
          return _context.Doctor.Any(e => e.Id == id);
        }

        private bool UserNameAvailable(string username)
        {
            if (_userManager.Users.All(m => m.NormalizedUserName != username.ToUpper()))
                return true;

            else
                return false;
        }

        private string GenerateUsername(string firstName, string lastName)
        {
            int counter = 1;
            string username = string.Empty;
            do
            {
                username = firstName + lastName.Substring(0, counter);
                counter++;
            } while (!UserNameAvailable(username) && counter <= lastName.Count());

            if(counter > lastName.Count())
            {
                Random random = new Random();
                int randomNum = 0;
                do
                {
                    randomNum = random.Next(10, 999);
                    username = firstName + lastName.Substring(0, 1) + randomNum;
                }while(!UserNameAvailable(username));
                
            }

            return username;
        }

        private List<WebAppUser> GetAdministrators()
        {
            List<WebAppUser> administrators = new List<WebAppUser>();
            var AllUsers = _userManager.Users.ToList();

            foreach(WebAppUser user in AllUsers)
            {
                if(_userManager.IsInRoleAsync(user, "Admin").Result ||
                    _userManager.IsInRoleAsync(user, "SuperUser").Result)
                {
                    administrators.Add(user);
                }
            }

            return administrators;
        }

        public IActionResult SystemUsers()
        {

            return View(_userManager.Users.Include(x=>x.PatientProfile).Include(x=>x.Doctor).ToList());
        }
    }
}
