using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Services;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestBibliothequeDotnet8.Controllers
{
    public class ReservationsController : Controller
    {

        private readonly IUsagers _usagersService;
        private readonly ILivres _livresService;
        private readonly IReservations _reservationsService;

        public ReservationsController(IUsagers usagersService, ILivres livresService, IReservations reservationsService)
        {
            _usagersService = usagersService;
            _livresService = livresService;
            _reservationsService = reservationsService;
        }       
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationsService.ObtenirReservationsAvecDisponibilite();
            return View(reservations);
        }

        public async Task<IActionResult> Ajouter()
        {
            var model = new ReservationViewModel
            {
                DateDebut = DateTime.Now.Date,
                DatePrevue = DateTime.Now.AddDays(7).Date
            };

            var (livres, usagers) = await ObtenirLivresEtUsagers();

            model.Livres = livres;
            model.Usagers = usagers;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter(ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reservation = new Reservations
                    {
                        DateDebut = model.DateDebut,
                        DateRetourEstimee = model.DatePrevue,
                        IDUsager = model.IdUsager,
                        IDLivre = model.IdLivre,
                        Annuler = false
                    };

                    await _reservationsService.AddAsync(reservation);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }

            var (livres, usagers) = await ObtenirLivresEtUsagers();
            model.Livres = livres;
            model.Usagers = usagers;

            return View(model);
        }

        public async Task<IActionResult> Modifier(Guid id)
        {
            var reservation = await _reservationsService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var model = new ReservationViewModel
            {
                IdReservation = reservation.ID,
                IdLivre = reservation.IDLivre,
                IdUsager = reservation.IDUsager,
                DateDebut = reservation.DateDebut,
                DatePrevue = reservation.DateRetourEstimee
            };

            var (livres, usagers) = await ObtenirLivresEtUsagers();

            model.Livres = livres;
            model.Usagers = usagers;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modifier(Guid id, ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reservation = new Reservations
                    {
                        ID = model.IdReservation,
                        DateDebut = model.DateDebut,
                        DateRetourEstimee = model.DatePrevue,
                        IDUsager = model.IdUsager,
                        IDLivre = model.IdLivre
                    };

                    await _reservationsService.UpdateAsync(reservation); 
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                }
            }

            var (livres, usagers) = await ObtenirLivresEtUsagers();
            model.Livres = livres;
            model.Usagers = usagers;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmerAnnulation(Guid id)
        {
            try
            {
                var reservation = await _reservationsService.GetByIdAsync(id);
                if (reservation == null)
                    return NotFound();

                var viewModel = new ReservationViewModel
                {
                    IdReservation = reservation.ID,
                    UsagerNom = reservation.Usager.Nom + " " + reservation.Usager.Prenoms,
                    LivreTitre = reservation.Livre.Titre,
                    DateDebut = reservation.DateDebut,
                    DatePrevue = reservation.DateRetourEstimee,
                    Annuler = reservation.Annuler,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                GestionErreurs.GererErreur(ex, this);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Annuler(Guid idReservation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _reservationsService.AnnulerReservation(idReservation);
                    return RedirectToAction("Index", "Reservations");
                }
                catch (Exception ex)
                {
                    GestionErreurs.GererErreur(ex, this);
                    return RedirectToAction("Index", "Reservations");
                }
            }

            return RedirectToAction("Index", "Reservations");
        }

        public async Task<IActionResult> Rechercher(string searchQuery)
        {
            var reservations = await _reservationsService.ObtenirReservations();
            return View(reservations);
        }

        #region methodes privées
        private async Task<(List<SelectListItem> Livres, List<SelectListItem> Usagers)> ObtenirLivresEtUsagers()
        {
            var livres = await _livresService.GetAllAsync() ?? new List<Livres>();
            var usagers = await _usagersService.GetAllAsync() ?? new List<Usagers>();

            var livresSelectList = livres.Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Titre
            }).ToList();

            var usagersSelectList = usagers.Select(u => new SelectListItem
            {
                Value = u.ID.ToString(),
                Text = u.Nom + " " + u.Prenoms
            }).ToList();

            return (livresSelectList, usagersSelectList);
        }

        #endregion
    }
}
