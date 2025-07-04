using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestBibliothequeDotnet8.Controllers
{
    public class UsagersController : Controller
    {
        private readonly IUsagers _usagersService;
        private readonly IRetours _retourService;

        public UsagersController(IUsagers usagersService, IRetours retourService)
        {
            _usagersService = usagersService;
            _retourService = retourService;
        }

        public async Task<IActionResult> Index()
        {
            var usagers = await _usagersService.GetAllAsync();
            return View(usagers);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var usager = await _usagersService.GetByIdAsync(id);
            if (usager == null) return NotFound();

            var empruntsActifs = await _retourService.ObtenirEmpruntsActif(id);

            var viewModel = new UsagerDetailsViewModel
            {
                UsagerId = usager.ID,
                Nom = usager.Nom,
                Prenoms = usager.Prenoms,
                Courriel = usager.Courriel,
                Telephone = usager.Telephone,
                Emprunts = empruntsActifs
            };

            return View(viewModel);
        }
       
        public IActionResult Ajouter()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Usagers usager)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usagersService.AddAsync(usager);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            return View(usager);
        }
       
        public async Task<IActionResult> Modifier(Guid id)
        {
            var usager = await _usagersService.GetByIdAsync(id);
            if (usager == null)
            {
                return NotFound();
            }
            return View(usager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Usagers usager)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usagersService.UpdateAsync(usager);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            return View(usager);
        }

        public async Task<IActionResult> Supprimer(Guid id)
        {
            var usager = await _usagersService.GetByIdAsync(id);
            if (usager == null)
            {
                return NotFound();
            }

            return View(usager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirmation(Guid id)
        {
            try
            {
                await _usagersService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
                var usager = await _usagersService.GetByIdAsync(id);
                return View("Supprimer", usager);
            }
        }

    }
}
