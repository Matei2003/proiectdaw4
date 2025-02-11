﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using proiectdaw4.Data;
using proiectdaw4.Model;

namespace proiectdaw4.Controllers
{

    [Route("[controller]/[action]/{id?}")]
    public class RentingController : Controller
    {
        static int idGlobal;

        BdContext _context;
        public List<Proprietate> Proprietati { get; set; }

        public RentingController(BdContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public IActionResult ListaProprietati(string search)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                var proprietati = _context.Proprietati.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    proprietati = proprietati.Where(p => p.Name.Contains(search));
                }

                return View(proprietati.ToList());
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AdaugaProprietati()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                return View();
            }

            return RedirectToAction("Login", "Account");

        }

        [HttpGet]
        public IActionResult Inchiriaza(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var proprietate = _context.Proprietati.FirstOrDefault(p => p.Id == id);
            if (proprietate == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inchiriere = new Inchiriere
            {
                Proprietate = proprietate,
                ProprietateId = proprietate.Id,
                UserId = user.Id,
                DataInceput = DateOnly.FromDateTime(DateTime.Now),
                DataFinal = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                NumarZile = 0,
                PretTotal = 0
            };

            return View(inchiriere);
        }

        [HttpPost]
        public async Task<IActionResult> Inchiriaza(Inchiriere inchiriere)
        {
            
                if (inchiriere == null)
                {
                    ModelState.AddModelError(string.Empty, "Inchirierea nu a fost completata corect.");
                    return View(inchiriere);
                }

                var proprietate = await _context.Proprietati.FirstOrDefaultAsync(p => p.Id == inchiriere.ProprietateId);
                if (proprietate == null)
                {
                    ModelState.AddModelError(string.Empty, "Proprietatea nu a fost gasita.");
                    return View(inchiriere);
                }

                inchiriere.NumarZile = (inchiriere.DataFinal.ToDateTime(new TimeOnly()) - inchiriere.DataInceput.ToDateTime(new TimeOnly())).Days;
                if (inchiriere.NumarZile <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Data de incheiere trebuie sa fie după data de inceput.");
                    return View(inchiriere);
                }

                inchiriere.PretTotal = proprietate.Price * inchiriere.NumarZile;

                var email = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                inchiriere.UserId = user.Id; 
                inchiriere.ProprietateId = proprietate.Id;

            var existaSuprapunere = await _context.Inchirieri.AnyAsync(i =>
                i.ProprietateId == inchiriere.ProprietateId &&
                ((i.DataInceput <= inchiriere.DataFinal && i.DataFinal >= inchiriere.DataInceput))
            );

            if (!existaSuprapunere)
            {
                var inchiriereNoua = new Inchiriere
                {
                    ProprietateId = proprietate.Id,
                    UserId = user.Id,
                    PretTotal = inchiriere.PretTotal,
                    NumarZile = inchiriere.NumarZile,
                    DataInceput = inchiriere.DataInceput,
                    DataFinal = inchiriere.DataFinal,
                    Proprietate = proprietate,
                    User = user
                };

                _context.Inchirieri.Add(inchiriereNoua);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListaProprietati");
            }

            return RedirectToAction("Error", "Home");      

        }


        [HttpGet]
        public async Task<JsonResult> VerificaDisponibilitate(int proprietateId, DateOnly dataInceput, DateOnly dataFinal)
        {
            var existaSuprapunere = await _context.Inchirieri.AnyAsync(i =>
                i.ProprietateId == proprietateId &&
                (
                    (i.DataInceput <= dataFinal && i.DataFinal >= dataInceput)
                )
            );

            return Json(new { disponibil = !existaSuprapunere });
        }



        [HttpPost]
        public async Task<IActionResult> AdaugaProprietate(string Name, string Description, string Type, decimal Price)
        {
            if (ModelState.IsValid)
            {
                string email = HttpContext.Session.GetString("Email");

                var proprietate = new Proprietate
                {
                    Name = Name,
                    Description = Description,
                    Type = Type,
                    Price = Price,
                    UserEmail = email
                };

                _context.Proprietati.Add(proprietate);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListaProprietati");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var proprietate = await _context.Proprietati.FindAsync(id);

            if (proprietate == null)
            {
                return NotFound();
            }

            var userEmail = HttpContext.Session.GetString("Email");

            if (proprietate.UserEmail != userEmail)
            {
                return Forbid();
            }

            _context.Proprietati.Remove(proprietate);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListaProprietati");
        }



        
    }
}
