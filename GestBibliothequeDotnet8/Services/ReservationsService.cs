using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.EntityFrameworkCore;

namespace GestBibliothequeDotnet8.Services
{
    public class ReservationsService : IReservations
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILivres _livresService;
        private readonly IRecherche<Reservations> _rechercheReservation;
        public ReservationsService(IUnitOfWork unitOfWork, ILivres livres, IRecherche<Reservations> rechercheReservation)
        {
            _unitOfWork = unitOfWork;
            _livresService = livres;
            _rechercheReservation = rechercheReservation;
        }

        public async Task AddAsync(Reservations reservation)
        {
            ValidationService.VerifierNull(reservation, nameof(reservation), "La réservation");

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AnnulerReservation(Guid idReservation)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(idReservation);
            ValidationService.EnregistrementNonTrouve(reservation, "Reservations", idReservation);

            if (reservation.Annuler)
                throw new KeyNotFoundException(string.Format(ErreurMessage.ReservationAnnulee));

            if (reservation.Emprunt != null)
                throw new InvalidOperationException(string.Format(ErreurMessage.ErreurSuppressionEntiteLiee, "une réservation", "emprunts"));

            reservation.Annuler = true;
            await _unitOfWork.Reservations.UpdateAsync(reservation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.Reservations.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public Task<IEnumerable<Reservations>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Reservations> GetByIdAsync(Guid id)
        {
            var reservation = await _rechercheReservation.GetAll()
                                                        .Include(r => r.Livre)    
                                                        .Include(r => r.Usager)     
                                                        .FirstOrDefaultAsync(r => r.ID == id);

            ValidationService.EnregistrementNonTrouve(reservation, "Reservations", id);

            return reservation;
        }


        public Task<IEnumerable<ReservationViewModel>> ObtenirReservationsAvecDisponibilite(Guid? usagerId = null)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ReservationViewModel>> ObtenirReservationsAvecDisponibilite()
        {
            try
            {
                var reservationsActivesQuery = _rechercheReservation.GetAll()
                    .Include(r => r.Livre)
                    .Include(r => r.Usager)
                    .Where(r => r.Emprunt == null && !r.Annuler);

                var reservationsActives = await reservationsActivesQuery.ToListAsync();

                var viewModel = new List<ReservationViewModel>();

                foreach (var r in reservationsActives)
                {
                    var livreDisponible = await _livresService.EstDisponible(r.IDLivre);
                    var disponibilite = livreDisponible ? true : false;

                    viewModel.Add(new ReservationViewModel
                    {
                        IdReservation = r.ID,
                        LivreTitre = r.Livre.Titre,
                        UsagerNom = r.Usager.Nom + " " + r.Usager.Prenoms,
                        DateDebut = r.DateDebut,
                        DatePrevue = r.DateRetourEstimee,
                        Annuler = r.Annuler,
                        EstDisponible = disponibilite  // Ajout de la disponibilité ici
                    });
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Réservations actives"), ex);
            }
        }


        public async Task UpdateAsync(Reservations reservation)
        {
            ValidationService.VerifierNull(reservation, nameof(reservation), "La réservation");

            var reservationAModifier = await _unitOfWork.Reservations.GetByIdAsync(reservation.ID);
            ValidationService.EnregistrementNonTrouve(reservationAModifier, "Reservations", reservation.ID);

            if (reservationAModifier.Annuler)
                throw new KeyNotFoundException(string.Format(ErreurMessage.ReservationAnnulee));

            await _unitOfWork.Reservations.UpdateAsync(reservation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ReservationViewModel>> ObtenirReservations(Guid? usagerId = null)
        {
            try
            {
                var reservationsActivesQuery = _rechercheReservation.GetAll()
                .Include(r => r.Livre)
                .Include(r => r.Usager)
                .Where(r => r.Emprunt == null && !r.Annuler);

                if (usagerId.HasValue)
                {
                    reservationsActivesQuery = reservationsActivesQuery.Where(r => r.IDUsager == usagerId.Value);
                }

                var reservationsActives = await reservationsActivesQuery.ToListAsync();

                var viewModel = reservationsActives.Select(r => new ReservationViewModel
                {
                    IdReservation = r.ID,
                    LivreTitre = r.Livre.Titre,
                    UsagerNom = r.Usager.Nom + " " + r.Usager.Prenoms,
                    DateDebut = r.DateDebut,
                    DatePrevue = r.DateRetourEstimee,
                    Annuler = r.Annuler,
                }).ToList();

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Réservations actives"), ex);
            }
        }

    }
}
