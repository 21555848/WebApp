﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly WebAppContext _context;

        public AppointmentsController(WebAppContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var webAppContext = _context.Appointment.Include(a => a.Doctor);
            return View(await webAppContext.ToListAsync());
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
        public IActionResult ClinicVisit()
        {
            return View();
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

            if (ModelState.IsValid)
            {
                _context.Add(ap);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

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

        public async Task<IActionResult> Confirm(int? id)
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

            ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && appointment.DoctorId != null)
            {
                appointment.Approved = true;

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

            ViewData["NotConfirmed"] = "Appointment Not Yet Confirmed.";
            ViewData["DoctorId"] = new SelectList(_context.Doctor, "Id", "LastName");
            return View(appointment);
        }

        [HttpGet]
        public JsonResult GetDateTimes(DateTime fDate)
        {
            DateTime converTed = Convert.ToDateTime(fDate.ToString("yyyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US")));

            var dateOnly = DateOnly.FromDateTime(converTed);
            //string apQuery = "SELECT * FROM Appointment WHERE Date LIKE " + dateOnly;
            List<Appointment> appointmentsOnDay = _context.Appointment.Where(x => x.Date == dateOnly).ToList();
            // List<Appointment> appointmentsOnDay = _context.Appointment.FromSqlRaw(apQuery).ToList();

            int numDoctors = _context.Doctor.ToList().Count();

            List<string> availableTimes = new List<string>();

            TimeOnly timeCounter = new TimeOnly(08, 00, 00);

            TimeOnly timeLimit = new TimeOnly(16, 00, 00);

            while (timeCounter <= timeLimit)
            {
                var ap = appointmentsOnDay.Where(c => c.Time == timeCounter).ToList();
                //string SqlQuery = "SELECT * FROM Appointment WHERE Date LIKE" + dateOnly;
                //var result = _context.Appointment.FromSqlRaw(SqlQuery).ToList();
                if (ap.Count() <= numDoctors)
                    availableTimes.Add(timeCounter.ToString("HH:mm"));

                timeCounter = timeCounter.AddHours(1);
            }

            return Json(availableTimes);
        }

    }
}
