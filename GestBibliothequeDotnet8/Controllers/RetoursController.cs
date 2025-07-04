using AspNetCoreGeneratedDocument;
using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestBibliothequeDotnet8.Controllers
{
    public class RetoursController : Controller
    {
        private readonly IRetours _retoursService;
        private readonly IEmprunts _empruntsService;
        private readonly ILivres _livresService;
        private readonly IUsagers _usagersService;

        public RetoursController(IRetours retoursService, IEmprunts empruntsService, ILivres livresService, IUsagers usagersService)
        {
            _retoursService = retoursService;
            _empruntsService = empruntsService;
            _livresService = livresService;
            _usagersService = usagersService;
        }
        public async Task<IActionResult> EmpruntActif()
        {
            var viewModel = await _retoursService.ObtenirEmpruntsActif();
            return View(viewModel);
        }
        public async Task<IActionResult> EmpruntInActif()
        {
            var viewModel = await _retoursService.ObtenirEmpruntsInActif();
            return View(viewModel);
        }
        public async Task<IActionResult> RechercherEmprunt(string recherche)
        {
            var empruntsActifs = await _retoursService.ObtenirEmpruntsActif();

            var empruntsFiltres = _retoursService.FiltrerEmpruntsParRecherche(empruntsActifs, recherche);
            ViewData["Recherche"] = recherche;

            return View(empruntsFiltres);
        }
        public async Task<IActionResult> Ajouter(Guid empruntId)
        {

            var emprunt = await _empruntsService.GetByIdAsync(empruntId);
            if (emprunt == null)
            {
                return NotFound();
            }

            var livre = await _livresService.GetByIdAsync(emprunt.IDLivre);
            var usager = await _usagersService.GetByIdAsync(emprunt.IDUsager);

            if (livre == null || usager == null)
            {
                return BadRequest("Les informations du livre ou de l'emprunteur sont manquantes.");
            }

            var viewModel = new RetourViewModel
            {
                IDEmprunt = empruntId,
                LivreTitre = livre.Titre,
                UsagerNom = usager.Nom + " " + usager.Prenoms,
                DateRetour = DateTime.Now  
            };

            return View(viewModel);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Retours retour)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _retoursService.AddAsync(retour);
                    return RedirectToAction("Index", "Emprunts");
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
                var emprunt = await _empruntsService.GetByIdAsync(retour.IDEmprunt);
                var livre = await _livresService.GetByIdAsync(emprunt.IDLivre);
                var usager = await _usagersService.GetByIdAsync(emprunt.IDUsager);
                
                var viewModel = new RetourViewModel
                {
                    LivreTitre = livre.Titre,
                    UsagerNom = usager.Nom + " " + usager.Prenoms,
                };

                return View(viewModel);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var retour = await _retoursService.GetByIdAsync(id);
            if (retour == null)
            {
                return NotFound();
            }

            return View(retour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Retours retours)
        {
            if (retours.DateRetour == default)
            {
                ModelState.AddModelError("", "La date de retour ne peut pas être invalide.");
                return View();
            }

            try
            {
                await _retoursService.UpdateAsync(retours);
                return RedirectToAction("Index", "Emprunts");
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
            }
            return View();
        }
    }
}
