using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class AppointmentsController : Controller
    {
        private readonly WebAppContext _context;
        private readonly UserManager<WebAppUser> _userManager;
        private readonly SignInManager<WebAppUser> _signInManager;

        public AppointmentsController(WebAppContext context,
            UserManager<WebAppUser> userManager,
            SignInManager<WebAppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var webAppContext = _context.Appointment.Include(a => a.Doctor);
            return View(await webAppContext.ToListAsync());
        }

        public IActionResult MyAppointments()
        {
            return View();
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Id");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,CellNo,AlternateCell,EmailAddress,Date,Time,Approved,PIN,PatientId,DoctorId,Type")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Id", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Id", appointment.DoctorId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,CellNo,AlternateCell,EmailAddress,Date,Time,Approved,PIN,PatientId,DoctorId,Type")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Id", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appointment == null)
            {
                return Problem("Entity set 'WebAppContext.Appointment'  is null.");
            }
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.Id == id);
        }

        //GET: Appointments/Book
        [AllowAnonymous]
        public IActionResult ClinicVisit()
        {
            ViewData["AppointmentType"] = "Clinic Visit";
            if (_signInManager.IsSignedIn(User))
            {
                if(_userManager.IsInRoleAsync(CurrentUser(), "Default").Result)
                {
                    if (CurrentUser != null)
                    {
                        var patientProfile = GetPatientProfile();
                        var user = CurrentUser();
                        BookingModel model = new BookingModel
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            CellNo = patientProfile.CellNo,
                            AlternateCell = patientProfile.AlternateCell,
                            EmailAddress = patientProfile.EmailAddress
                        };

                        return View("Book", model);
                    }
                }

                ViewData["AppointmentType"] = "Clinic Visit - Admin";
                return View("Book");

            }


            return View("Book");

        }

        //POST: Appointments/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClinicVisit([Bind("FirstName, LastName, CellNo, AlternateCell, EmailAddress, Date, Time")] BookingModel bm)
        {
            Random rand = new Random();
            int ranNum = rand.Next(1000, 9999);

            var ap = new Appointment();
            ap.FirstName = bm.FirstName;
            ap.LastName = bm.LastName;
            ap.CellNo = bm.CellNo;
            ap.AlternateCell = bm.AlternateCell;
            ap.EmailAddress = bm.EmailAddress;
            ap.Date = DateOnly.FromDateTime(bm.Date);
            ap.Time = TimeOnly.FromDateTime(bm.Time);
            ap.PIN = ranNum;
            ap.Type = AppointmentType.ClinicVisit;
            ap.Approved = false;

            if (ModelState.IsValid)
            {
                _context.Add(ap);
                await _context.SaveChangesAsync();
                EmailConfig email = new EmailConfig();
                string body = string.Empty;
                using (var reader = new StreamReader(Path.GetFullPath("EmailTemplates/Appointment.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{AppointmentType}", "Clinic Visit");
                body = body.Replace("{User}", ap.FirstName);
                body = body.Replace("{Reference}", ap.Id.ToString());
                body = body.Replace("{PIN}", ap.PIN.ToString());
                body = body.Replace("{Date}", ap.Date.ToLongDateString());
                body = body.Replace("{Time}", ap.Time.ToString());
                body = body.Replace("{FullName}", ap.FirstName + " " + ap.LastName);

                email.SendEmail(ap.EmailAddress, "Clinic Visit Appointment Details Ref#: " + ap.Id, body);
                ViewData["DateTime"] = ap.Date.ToDateTime(ap.Time).ToString("dd MMMM yyyy HH:mm",
                    CultureInfo.CreateSpecificCulture("en-US"));
                return RedirectToAction("MyAppointment", ap);
            }
            ViewData["AppointmentType"] = "Clinic Visit";
            return View("Book", bm);
        }

        [AllowAnonymous]
        public IActionResult CheckBooking()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckBooking(MyBookingModel m)
        {
            var ap = _context.Appointment.Include(d => d.Doctor).FirstOrDefault(a => a.Id == m.Reference);

            if (ap == null)
            {
                return View();
            }
            if (ap.PIN == m.PIN)
            {
                ViewData["DateTime"] = ap.Date.ToDateTime(ap.Time).ToString("dd MMMM yyyy HH:mm",
                    CultureInfo.CreateSpecificCulture("en-US"));


                return View("MyAppointment", ap);
            }

            return View();

        }

        [Authorize(Roles = "Default")]
        public IActionResult HomeCall()
        {
            var patientProfile = GetPAtientProfileWithAddress();
            var user = CurrentUser();
            BookingModel model = new BookingModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CellNo = patientProfile.CellNo,
                AlternateCell = patientProfile.AlternateCell,
                EmailAddress = patientProfile.EmailAddress,
                StreetAddress = patientProfile.PatientAddress.StreetAddress,
                Address2 = patientProfile.PatientAddress.Address2,
                Suburb = patientProfile.PatientAddress.Suburb,
                Province = patientProfile.PatientAddress.Province
            };
            ViewData["AppointmentType"] = "Home Call";
            return View("Book", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Default")]
        public async Task<IActionResult> HomeCall(BookingModel appointment)
        {
            if (ModelState.IsValid)
            {
                Random rand = new Random();
                int ranNum = rand.Next(1000, 9999);
                var pp = GetPAtientProfileWithAddress();
                Appointment ap = new Appointment
                {
                    FirstName = appointment.FirstName,
                    LastName = appointment.LastName,
                    CellNo = appointment.CellNo,
                    AlternateCell = appointment.AlternateCell,
                    EmailAddress = appointment.EmailAddress,
                    Date = DateOnly.FromDateTime(appointment.Date),
                    Time = TimeOnly.FromDateTime(appointment.Time),
                    PIN = ranNum,
                    Type = AppointmentType.OnlineConsultation,
                    PatientId = GetPatientId()
                };

                if (appointment.UseDifferentAddress)
                {
                    ap.StreetAddress = appointment.altStreetAddress;
                    ap.Address2 = appointment.altAddress2;
                    ap.Suburb = appointment.altSuburb;
                    ap.Province = appointment.altProvince;
                }
                else
                {
                    ap.StreetAddress = pp.PatientAddress.StreetAddress;
                    ap.Address2 = pp.PatientAddress.Address2;
                    ap.Suburb = pp.PatientAddress.Suburb;
                    ap.Province = pp.PatientAddress.Province;
                }

                _context.Add(ap);
                await _context.SaveChangesAsync();
                EmailConfig email = new EmailConfig();
                string body = string.Empty;
                using (var reader = new StreamReader(Path.GetFullPath("EmailTemplates/HomeAppointment.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{AppointmentType}", "Home Call");
                body = body.Replace("{User}", ap.FirstName);
                body = body.Replace("{Reference}", ap.Id.ToString());
                body = body.Replace("{PIN}", ap.PIN.ToString());
                body = body.Replace("{Date}", ap.Date.ToLongDateString());
                body = body.Replace("{Time}", ap.Time.ToString());
                body = body.Replace("{FullName}", ap.FirstName + " " + ap.LastName);
                body = body.Replace("{StreetAddress}", ap.StreetAddress);
                body = body.Replace("{Address2}", ap.Address2);
                body = body.Replace("{Suburb}", ap.Suburb);
                body = body.Replace("{Province}", ap.Province);

                email.SendEmail(ap.EmailAddress, "Dr Booking Home Call Appointment Details Ref#: " + ap.Id, body);
                ViewData["DateTime"] = ap.Date.ToDateTime(ap.Time).ToString("dd MMMM yyyy HH:mm",
                    CultureInfo.CreateSpecificCulture("en-US"));
                return View("MyAppointment", ap);
            }

            ViewData["AppointmentType"] = "Online Consultation";
            return View("Book", appointment);
        }

        public IActionResult MyAppointment()
        {
            return View();
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || _context == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.Approved)
            {
                return RedirectToAction(nameof(Update));
            }

            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
            return View(appointment);

        }

        //public async Task<IActionResult> Confirm(int? id)
        //{
        //    if (id == null || _context == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointment.FindAsync(id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    if (appointment.Approved)
        //    {
        //        return RedirectToAction(nameof(Update));
        //    }

        //    ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
        //    ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
        //    return View(appointment);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Confirm(int id, Appointment appointment)
        //{
        //    if (id != appointment.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid && appointment.DoctorId != null)
        //    {
        //        appointment.Approved = true;
        //        //appointment.Date = GetAppointmentDate(id);
        //        //appointment.Time = GetAppointmentTime(id);

        //        try
        //        {
        //            _context.Update(appointment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AppointmentExists(appointment.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Unconfirmed));
        //    }

        //    ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
        //    ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
        //    return View(appointment);
        //}
        [Authorize(Roles = "Admin,SuperUser")]
        public IActionResult Confirm(int? id)
        {
            if (id == null || _context == null)
            {
                return NotFound();
            }

            var appointment = _context.Appointment.FindAsync(id).Result;
            var doctors = _context.Doctor.Include(a => a.Appointments).Where(x => x.Active == true).ToList();
            List<DoctorMaintenanceModel> docList = new List<DoctorMaintenanceModel>();
            
           
            if (appointment == null)
            {
                return NotFound();
            }

            foreach (var doc in doctors)
            {
                var user = _userManager.FindByIdAsync(doc.WebAppUserId).Result;
                if (doc.Appointments.Count != 0)
                {
                    List<Appointment> appointmentsOnDay = new List<Appointment>();
                    int timeCount = 0;
                    foreach (Appointment app in doc.Appointments)
                    {
                        if (app.Date == appointment.Date)
                        {
                            appointmentsOnDay.Add(app);
                        }
                    }

                    foreach(var app in appointmentsOnDay)
                    {
                        if(app.Time != appointment.Time)
                        {
                            timeCount++;
                            //docList.Add(new DoctorMaintenanceModel
                            //{
                            //    DoctorId = doc.Id,
                            //    FirstName = user.FirstName,
                            //    LastName = user.LastName
                            //});
                        }
                    }
                    if (appointmentsOnDay.Count != 0)
                    {
                        if (timeCount + 1 < appointmentsOnDay.Count)
                        {
                            docList.Add(new DoctorMaintenanceModel
                            {
                                DoctorId = doc.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName
                            });
                        }
                    }

                    else if(appointmentsOnDay.Count == 0)
                    {
                        docList.Add(new DoctorMaintenanceModel
                        {
                            DoctorId = doc.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        });
                    }
                        
                }

                else
                {
                    docList.Add(new DoctorMaintenanceModel
                    {
                        DoctorId = doc.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });
                }
                
            }

            BookingModel bm = new BookingModel
            {
                Id = appointment.Id,
                FirstName = appointment.FirstName,
                LastName = appointment.LastName,
                CellNo = appointment.CellNo,
                AlternateCell = appointment.AlternateCell,
                EmailAddress = appointment.EmailAddress,
                Date = appointment.Date.ToDateTime(appointment.Time),
                Time = appointment.Date.ToDateTime(appointment.Time)
            };


            ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
            ViewData["DoctorId"] = new SelectList(docList, "DoctorId", "LastName");
            return View(bm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Confirm(int id, BookingModel bm)
        {
            if (ModelState.IsValid && bm.DoctorId != null)
            {
                var ap = _context.Appointment.FindAsync(id).Result;
                if (ap == null)
                {
                    return NotFound();
                }

                ap.DoctorId = bm.DoctorId;
                ap.Approved = true;

                var doc = _context.Doctor.Include(x=>x.Suite).FirstOrDefault(x => x.Id == ap.DoctorId);
                var docDetails = _userManager.FindByIdAsync(doc.WebAppUserId).Result;
                string docName = docDetails.FirstName.Substring(0, 1) + " " + docDetails.LastName;
                string apType = string.Empty;

                if (ap.Type == AppointmentType.ClinicVisit)
                    apType = "Clinic Visit";
                else if (ap.Type == AppointmentType.HomeCall)
                    apType = "Home Call";
                else
                    apType = "Online Consultation";

                EmailConfig email = new EmailConfig();
                string body = string.Empty;
               

                try
                {
                    _context.Update(ap);
                    await _context.SaveChangesAsync();
                    if (ap.Type == AppointmentType.HomeCall)
                    {
                        using (var reader = new StreamReader(Path.GetFullPath("EmailTemplates/HomeAppointmentConfirmed.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{AppointmentType}", apType);
                        body = body.Replace("{User}", ap.FirstName);
                        body = body.Replace("{Date}", ap.Date.ToLongDateString());
                        body = body.Replace("{Time}", ap.Time.ToString());
                        body = body.Replace("{FullName}", ap.FirstName + " " + ap.LastName);
                        body = body.Replace("{StreetAddress}", ap.StreetAddress);
                        body = body.Replace("{Address2}", ap.Address2);
                        body = body.Replace("{Suburb}", ap.Suburb);
                        body = body.Replace("{Province}", ap.Province);
                        body = body.Replace("{Doctor}", docName);
                    }
                    else if(ap.Type == AppointmentType.ClinicVisit)
                    {
                        using (var reader = new StreamReader(Path.GetFullPath("EmailTemplates/AppointmentConfirmed.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{AppointmentType}", "Home Call");
                        body = body.Replace("{User}", ap.FirstName);
                        body = body.Replace("{Date}", ap.Date.ToLongDateString());
                        body = body.Replace("{Time}", ap.Time.ToString());
                        body = body.Replace("{Suite}", doc.Suite.Name);
                        body = body.Replace("{Doctor}", docName);
                    }
                    else
                    {
                        return RedirectToAction(nameof(SetupMeeting), new { Id = ap.Id });
                    }
                    email.SendEmail(ap.EmailAddress, "CONFIRMED: Dr Booking" + apType + "Appointment Details Ref#: " + ap.Id, body);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(ap.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Unconfirmed));
            }

            ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
            return View(bm);
        }

        [Authorize(Roles ="Admin,SuperUser")]
        public IActionResult SetupMeeting(int Id)
        {
            var ap = _context.Appointment.Include(x => x.Doctor).FirstOrDefault(x => x.Id == Id);
            if(ap == null)
            {
                return NotFound();
            }
            var doc = _userManager.FindByIdAsync(ap.Doctor.WebAppUserId).Result;

            OnlineMeetingModel onlineMeetingModel = new OnlineMeetingModel
            {
                Id = ap.Id,
                FullName = ap.FirstName + " " + ap.LastName,
                Doctor = doc.FirstName.Substring(0, 1) + " " + doc.LastName,
                Date = ap.Date.ToLongDateString() + ap.Time
            };
            return View(onlineMeetingModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupMeeting(int Id, OnlineMeetingModel mm)
        {
            if(Id == null || mm.Link == null)
            {
                return NotFound();
            }

            var ap = await _context.Appointment.FindAsync(Id);

            ap.Link = mm.Link;
            _context.Update(ap);
            await _context.SaveChangesAsync();

            EmailConfig email = new EmailConfig();
            string body = string.Empty;

            using (var reader = new StreamReader(Path.GetFullPath("EmailTemplates/OnlineConfirmed.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{AppointmentType}", "Online Consultation");
            body = body.Replace("{User}", ap.FirstName);
            body = body.Replace("{Date}", ap.Date.ToLongDateString() + " " + ap.Time);
            body = body.Replace("{Doctor}", mm.Doctor);
            body = body.Replace("{MeetingLink}", mm.Link);
            email.SendEmail(ap.EmailAddress, "CONFIRMED: Dr Booking Online Consultation Appointment Details Ref#: " + ap.Id, body);
            return RedirectToAction(nameof(Unconfirmed));
        }

        //Returns a list of available times on the date selected by the user as a JSon object
        [HttpGet]
        public JsonResult GetDateTimes(DateTime fDate)
        {
            DateTime converTed = Convert.ToDateTime(fDate.ToString("yyyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US")));

            var dateOnly = DateOnly.FromDateTime(converTed);
            //string apQuery = "SELECT * FROM Appointment WHERE Date LIKE " + dateOnly;
            List<Appointment> appointmentsOnDay = _context.Appointment.Where(x => x.Date == dateOnly).ToList();
            // List<Appointment> appointmentsOnDay = _context.Appointment.FromSqlRaw(apQuery).ToList();
            
            int numDoctors = _context.Doctor.Where(d=>d.Active).ToList().Count();

            List<string> availableTimes = new List<string>();

            TimeOnly timeCounter = new TimeOnly(08, 00, 00);

            TimeOnly timeLimit = new TimeOnly(16, 00, 00);

            if(fDate < DateTime.Today)
            {
                availableTimes.Add("Selected Date In Past");
                return Json(availableTimes);
            }

            while (timeCounter <= timeLimit)
            {
                var ap = appointmentsOnDay.Where(c => c.Time == timeCounter).ToList();
                //string SqlQuery = "SELECT * FROM Appointment WHERE Date LIKE" + dateOnly;
                //var result = _context.Appointment.FromSqlRaw(SqlQuery).ToList();
                if (ap.Count() < numDoctors)
                    availableTimes.Add(timeCounter.ToString("HH:mm"));

                timeCounter = timeCounter.AddHours(1);
            }

            return Json(availableTimes);
        }


        [Authorize(Roles ="Default")]
        public IActionResult OnlineConsultation()
        {
            var patientProfile = GetPatientProfile();
            var user = CurrentUser();
            BookingModel model = new BookingModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CellNo = patientProfile.CellNo,
                AlternateCell = patientProfile.AlternateCell,
                EmailAddress = patientProfile.EmailAddress
            };
            ViewData["AppointmentType"] = "Online Consultation";
            return View("Book", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Default")]
        public async Task<IActionResult> OnlineConsultation(BookingModel appointment)
        {
            if (ModelState.IsValid)
            {
                Random rand = new Random();
                int ranNum = rand.Next(1000, 9999);

                Appointment ap = new Appointment
                {
                    FirstName = appointment.FirstName,
                    LastName = appointment.LastName,
                    CellNo = appointment.CellNo,
                    AlternateCell = appointment.AlternateCell,
                    EmailAddress = appointment.EmailAddress,
                    Date = DateOnly.FromDateTime(appointment.Date),
                    Time = TimeOnly.FromDateTime(appointment.Time),
                    PIN = ranNum,
                    Type = AppointmentType.OnlineConsultation,
                    PatientId = GetPatientId()
                };

                _context.Add(ap);
                await _context.SaveChangesAsync();
                ViewData["DateTime"] = ap.Date.ToDateTime(ap.Time).ToString("dd MMMM yyyy HH:mm",
                    CultureInfo.CreateSpecificCulture("en-US"));
                return View("MyAppointment", ap);
            }

            ViewData["AppointmentType"] = "Online Consultation";
            return View("Book", appointment);
        }

        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Unconfirmed()
        {
            var appointments = _context.Appointment.Where(x => x.Approved == false);
            return View(await appointments.ToListAsync());
        }

        //Returns a view of confirmed appointments
        [Authorize(Roles ="Admin,Doctor,SuperUser")]
        public async Task<IActionResult> Confirmed()
        {
            var appointments = _context.Appointment.Where(x => x.Approved == true).Include(x=>x.Doctor);
            List<ConfirmedAppointmentViewModel> appointmentVM = new List<ConfirmedAppointmentViewModel>();
            foreach(var appointment in appointments)
            {
                appointmentVM.Add(new ConfirmedAppointmentViewModel
                {
                    Id = appointment.Id,
                    FirstName = appointment.FirstName,
                    LastName = appointment.LastName,
                    CellNo = appointment.CellNo,
                    AlternateCell = appointment.AlternateCell,
                    EmailAddress = appointment.EmailAddress,
                    Date = appointment.Date,
                    Time = appointment.Time,
                    Doctor = _userManager.FindByIdAsync(appointment.Doctor.WebAppUserId).Result,
                    Type = appointment.Type
                });
            }
            return View(appointmentVM);
        }

        //Returns PatientProfile for user currently logged on
        private PatientProfile GetPatientProfile()
        {
            return _context.PatientProfile.FirstOrDefault(x => x.WebAppUserId == CurrentUser().Id);
        }

        //Retuens patient profile with address
        private PatientProfile GetPAtientProfileWithAddress()
        {
            return _context.PatientProfile.Include(x => x.PatientAddress).FirstOrDefault(x => x.WebAppUserId == CurrentUser().Id);
        }

        //Returns user currently logged on
        private WebAppUser CurrentUser()
        {
            return _userManager.FindByNameAsync(_userManager.GetUserName(User)).Result;
        }

        //Returns PatientProfile Id for user signed in
        private int GetPatientId()
        {
            return GetPatientProfile().Id;
        }

        //private async Task<DateOnly> GetAppointmentDate(int id)
        //{
        //    return _context.Appointment.FindAsync(id).Result.Date;
        //}

        //private TimeOnly GetAppointmentTime(int id)
        //{
        //    return _context.Appointment.Find(id).Time;
        //}
    }
}
