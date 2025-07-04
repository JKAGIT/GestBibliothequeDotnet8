using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestBibliothequeDotnet8.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly IUtilisateurs _utilisateursService;
        private readonly GenerateurMatriculeUnique _generateurMatricule;

        public UtilisateursController(IUtilisateurs utilisateursService, GenerateurMatriculeUnique generateurMatricule)
        {
            _utilisateursService = utilisateursService;
            _generateurMatricule = generateurMatricule;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var utilisateurs = await _utilisateursService.GetAllAsync();
            return View(utilisateurs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var utilisateur = await _utilisateursService.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            return View(utilisateur);
        }
        public async Task<IActionResult> Ajouter()
        {
            var utilisateur = new Utilisateurs
            {
                Matricule = await _generateurMatricule.GenererMatriculeUnique()
            };
            return View(utilisateur); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Utilisateurs utilisateur)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _utilisateursService.AddAsync(utilisateur);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            ViewBag.Utilisateurs = await _utilisateursService.GetAllAsync();
            return View(utilisateur);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var utilisateur = await _utilisateursService.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound(); 
            }
            return View(utilisateur); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Utilisateurs utilisateur)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _utilisateursService.UpdateAsync(utilisateur);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }

            return View(utilisateur); 
        }

        public async Task<IActionResult> Supprimer(Guid id)
        {
            var utilisateur = await _utilisateursService.GetByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirmation(Guid id)
        {
            try
            {
                await _utilisateursService.DeleteAsync(id);
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
