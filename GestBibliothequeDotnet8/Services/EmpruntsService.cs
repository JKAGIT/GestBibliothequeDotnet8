using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Services
{
    public class EmpruntsService : IEmprunts
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecherche<Emprunts> _recherche;
        private readonly ILivres _livresService;

        public EmpruntsService(IUnitOfWork unitOfWork, IRecherche<Emprunts> recherche, ILivres livresService)
        {
            _unitOfWork = unitOfWork;
            _recherche = recherche;
            _livresService = livresService;
        }

        public async Task AddAsync(Emprunts emprunt)
        {
            ValidationService.VerifierNull(emprunt, nameof(emprunt), "L'emprunt");

            await _livresService.MettreAJourStock(emprunt.IDLivre, -1);

            await _unitOfWork.Emprunts.AddAsync(emprunt);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Emprunts emprunt)
        {
            ValidationService.VerifierNull(emprunt, nameof(emprunt), "L'emprunt");

            var empruntAModifier = await _unitOfWork.Emprunts.GetByIdAsync(emprunt.ID);
            ValidationService.EnregistrementNonTrouve(empruntAModifier, "Emprunts", emprunt.ID);

            await _unitOfWork.Emprunts.UpdateAsync(emprunt);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid idEmprunt)
        {
            var empruntASupprimer = await _unitOfWork.Emprunts.GetByIdAsync(idEmprunt);
            ValidationService.EnregistrementNonTrouve(empruntASupprimer, "Emprunts", idEmprunt);

            await _livresService.MettreAJourStock(empruntASupprimer.IDLivre, 1);
            await _unitOfWork.Emprunts.DeleteAsync(idEmprunt);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Emprunts>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.Emprunts.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Livres"), ex);
            }
        }

        public async Task<Emprunts> GetByIdAsync(Guid idEmprunt)
        {
            var emprunt = await _unitOfWork.Emprunts.GetByIdAsync(idEmprunt);
            ValidationService.EnregistrementNonTrouve(emprunt, "Emprunts", idEmprunt);

            return emprunt;
        }

        public async Task<IEnumerable<Emprunts>> ObtenirEmpruntParUsager(Guid idUsager)
        {
            try
            {
                return await _recherche.FindAsync(e => e.IDUsager == idUsager);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Emprunts"), ex);
            }
        }

        public async Task AjouterEmpruntReservation(Guid idReservation)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(idReservation);
            ValidationService.EnregistrementNonTrouve(reservation, "Reservations", idReservation);

            if (reservation.Annuler)
                throw new KeyNotFoundException(string.Format(ErreurMessage.ReservationAnnulee));

            if (reservation.Emprunt != null)
                throw new InvalidOperationException(string.Format(ErreurMessage.EmpruntEffectue));

            // disponibilité du livre
            var livre = await _unitOfWork.Livres.GetByIdAsync(reservation.IDLivre);
            ValidationService.EnregistrementNonTrouve(livre, "Livres", reservation.IDLivre);
            if (livre.Stock <= 0)
                throw new InvalidOperationException(string.Format(ErreurMessage.StockEpuise));

            // Emprunt de la réservation
            var emprunt = new Emprunts
            {
                DateDebut = DateTime.Now,
                DateRetourPrevue = reservation.DateRetourEstimee,
                IDUsager = reservation.IDUsager,
                IDLivre = reservation.IDLivre,
                IDReservation = reservation.ID,
            };

            await _unitOfWork.Emprunts.AddAsync(emprunt);
            await _livresService.MettreAJourStock(reservation.IDLivre, -1);
            reservation.Emprunt = emprunt;

            await _unitOfWork.Reservations.UpdateAsync(reservation);
            await _unitOfWork.CompleteAsync();
        }
    }
}
