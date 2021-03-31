using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationApp.Data;
using ReservationApp.Models;

namespace ReservationApp.Controllers
{
    //[Authorize(Roles = "Student")]
    public class ReservationsController : Controller
    {
        public int reserveValid = 0;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ReservationsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        

        // GET: Reservations
        public async Task<ActionResult> Index()
        {
            
            if (User.IsInRole("Student"))
            {
                IdentityUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var reservations = _context.Reservation.Include(r => r.TypeReservation).ToList().Where(r => r.UserId == user.Id).OrderBy(r => r.Status); // include pour afficher le nom de type d'objet type
                return View(reservations);
            }
            else
            {
                var reservations = _context.Reservation.Include(r => r.TypeReservation).Include(r => r.User).ToList().OrderBy(r => r.Status);
                return View(reservations);
            }
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {

            var reservation = new Reservation();
            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);  // recuperer id de user en connection pour remplir la valeur de UserId comme proprietaire de cette reservation crée
            IEnumerable<TypeReservation> types = _context.TypeReservation.ToList(); // pour charger les types et les afficher sur input select
            ViewBag.Types = types;

            return View(reservation);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create( Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
        

            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Student"))
            {
                ViewBag.UserId = _userManager.GetUserId(HttpContext.User);  // recuperer id de user en connection pour remplir la valeur de UserId comme proprietaire de cette reservation crée
                IEnumerable<TypeReservation> types = _context.TypeReservation.ToList(); // pour charger les types et les afficher sur input select
                ViewBag.Types = types;
                return View("Edit", reservation);
            }
            else
            {
                ViewBag.UserId = _userManager.GetUserId(HttpContext.User);  // recuperer id de user en connection pour remplir la valeur de UserId comme proprietaire de cette reservation crée
                IEnumerable<TypeReservation> types = _context.TypeReservation.ToList(); // pour charger les types et les afficher sur input select
                ViewBag.Types = types;
                return View("EditA", reservation);
            }
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();

                    //// Counter:
                    //public IActionResult Status(int id)
                    //{

                    //    var reservation = _db.Reservations.Include(x => x.utitlisateur).FirstOrDefault(x => x.id_reservation == id);
                    //    reservation.Status = "Approve";
                    //    reservation.utitlisateur.Counter++;
                    //    _db.Reservations.Update(reservation);
                    //    _db.SaveChanges();

                    //    return RedirectToAction("Index");
                    //}

                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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

            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);  // recuperer id de user en connection pour remplir la valeur de UserId comme proprietaire de cette reservation crée
            IEnumerable<TypeReservation> types = _context.TypeReservation.ToList(); // pour charger les types et les afficher sur input select
            ViewBag.Types = types;
            return View(reservation);

        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
