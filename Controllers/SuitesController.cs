using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class SuitesController : Controller
    {
        private readonly WebAppContext _context;

        public SuitesController(WebAppContext context)
        {
            _context = context;
        }

        // GET: Suites
        public async Task<IActionResult> Index()
        {
              return View(await _context.Suite.ToListAsync());
        }

        // GET: Suites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suite == null)
            {
                return NotFound();
            }

            var suite = await _context.Suite
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suite == null)
            {
                return NotFound();
            }

            return View(suite);
        }

        // GET: Suites/Create
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult AddForDoc()
        {
            return View("Add");
        }

        // POST: Suites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddForDoc(SuiteMaintenanceModel suite, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Suite s = new Suite { Name = suite.SuiteName };
                _context.Add(s);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Add",suite);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SuiteMaintenanceModel suite)
        {
            if (ModelState.IsValid)
            {
                Suite s = new Suite { Name = suite.SuiteName };
                _context.Add(s);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suite);
        }


        // GET: Suites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suite == null)
            {
                return NotFound();
            }

            var suite = await _context.Suite.FindAsync(id);
            if (suite == null)
            {
                return NotFound();
            }
            return View(suite);
        }

        // POST: Suites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Suite suite)
        {
            if (id != suite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuiteExists(suite.Id))
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
            return View(suite);
        }

        // GET: Suites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suite == null)
            {
                return NotFound();
            }

            var suite = await _context.Suite
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suite == null)
            {
                return NotFound();
            }

            return View(suite);
        }

        // POST: Suites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suite == null)
            {
                return Problem("Entity set 'WebAppContext.Suite'  is null.");
            }
            var suite = await _context.Suite.FindAsync(id);
            if (suite != null)
            {
                _context.Suite.Remove(suite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuiteExists(int id)
        {
          return _context.Suite.Any(e => e.Id == id);
        }
    }
}
