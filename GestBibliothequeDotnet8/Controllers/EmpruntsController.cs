using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestBibliothequeDotnet8.Controllers
{
    public class EmpruntsController : Controller
    {
        private readonly IEmprunts _empruntsService;
        private readonly IUsagers _usagersService;
        private readonly ILivres _livresService;
        private readonly IRetours _retoursService;
        private readonly IReservations _reservationsService;

        public EmpruntsController(IEmprunts empruntsService, IUsagers usagersService, ILivres livresService, IRetours retoursService, IReservations reservationsService)
        {
            _empruntsService = empruntsService;
            _usagersService = usagersService;
            _livresService = livresService;
            _retoursService = retoursService;
            _reservationsService = reservationsService;
        }

        public async Task<IActionResult> Index(int pageActifs = 1, int pageInactifs = 1, int pageSize = 5)
        {
            var empruntsActifs = await _retoursService.ObtenirEmpruntsActif();
            var empruntsInactifs = await _retoursService.ObtenirEmpruntsInActif();

            var totalPagesActifs = (int)Math.Ceiling((double)empruntsActifs.Count() / pageSize);
            var totalPagesInactifs = (int)Math.Ceiling((double)empruntsInactifs.Count() / pageSize);

            ViewBag.PageActifs = pageActifs;
            ViewBag.TotalPagesActifs = totalPagesActifs;

            ViewBag.PageInactifs = pageInactifs;
            ViewBag.TotalPagesInactifs = totalPagesInactifs;
            ViewBag.PageSize = pageSize;

            var viewModel = new EmpruntsIndexViewModel
            {
                EmpruntsActifs = empruntsActifs.Skip((pageActifs - 1) * pageSize).Take(pageSize).ToList(),
                EmpruntsInactifs = empruntsInactifs.Skip((pageInactifs - 1) * pageSize).Take(pageSize).ToList()
            };

            return View(viewModel);
        }



        public async Task<IActionResult> Details(Guid id)
        {
            var emprunt = await _empruntsService.GetByIdAsync(id);
            if (emprunt == null) return NotFound();

            await ChargeAssociationEntite(emprunt);

            return View(emprunt);
        }

        public async Task<IActionResult> Ajouter()
        {
            await RemplirViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(Emprunts emprunts)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _empruntsService.AddAsync(emprunts);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }

            await RemplirViewBags();
            return View(emprunts);
        }

        [HttpGet]
        public async Task<IActionResult> EmprunterViaReservation(Guid idReservation)
        {
            var reservation = await _reservationsService.GetByIdAsync(idReservation);

            if (reservation == null || reservation.Livre == null || reservation.Usager == null)
            {
                return NotFound();
            }

            // modèle de vue avec les données de la réservation
            var model = new ReservationViewModel
            {
                IdReservation = reservation.ID,
                IdLivre = reservation.IDLivre,
                IdUsager = reservation.IDUsager,
                LivreTitre = reservation.Livre.Titre, 
                UsagerNom = $"{reservation.Usager.Nom} {reservation.Usager.Prenoms}", 
                DateDebut = reservation.DateDebut,
                DatePrevue = reservation.DateRetourEstimee,
                Livres = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = reservation.Livre.ID.ToString(),
                        Text = reservation.Livre.Titre
                    }
                },
                        Usagers = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = reservation.Usager.ID.ToString(),
                        Text = $"{reservation.Usager.Nom} {reservation.Usager.Prenoms}"
                    }
                }
                    };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmprunterViaReservation(ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _empruntsService.AjouterEmpruntReservation(model.IdReservation);
                    return RedirectToAction("Index", "Emprunts");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Une erreur est survenue lors de l'emprunt : {ex.Message}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var emprunt = await _empruntsService.GetByIdAsync(id);
            if (emprunt == null) return NotFound();

            await RemplirViewBags();

            return View(emprunt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Emprunts emprunt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _empruntsService.UpdateAsync(emprunt);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }
            await RemplirViewBags();

            return View(emprunt);
        }

        public async Task<IActionResult> Supprimer(Guid id)
        {
            var emprunt = await _empruntsService.GetByIdAsync(id);
            if (emprunt == null) return NotFound();
            return View(emprunt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerConfirmation(Guid id)
        {
            try
            {
                await _empruntsService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
            }
            return RedirectToAction(nameof(Index));
        }

        #region Private Methods

        //ViewBags avec Usagers et Livres pour les selections (combo)
        private async Task RemplirViewBags()
        {
            var usagers = await _usagersService.GetAllAsync();
            var livres = await _livresService.ObtenirLivresEnStock();

            ViewBag.Usagers = usagers.Select(u => new SelectListItem
            {
                Value = u.ID.ToString(),
                Text = $"{u.Nom} {u.Prenoms}"
            }).ToList();

            ViewBag.Livres = livres.Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Titre
            }).ToList();
        }

        // Charger les entités associées (Livre et Usager)
        private async Task ChargeAssociationEntite(Emprunts emprunt)
        {
            if (emprunt.Usager == null)
            {
                emprunt.Usager = await _usagersService.GetByIdAsync(emprunt.IDUsager);
            }

            if (emprunt.Livre == null)
            {
                emprunt.Livre = await _livresService.GetByIdAsync(emprunt.IDLivre);
            }
        }

        #endregion
    }
}
