using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.EntityFrameworkCore;

namespace GestBibliothequeDotnet8.Services
{
    public class RetoursService : IRetours
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmprunts _empruntsService;
        private readonly ILivres _livresService;
        private readonly IRecherche<Retours> _rechercheRetour;
        private readonly IRecherche<Emprunts> _rechercheEmprunt;

        public RetoursService(IUnitOfWork unitOfWork, ILivres livresService, IEmprunts empruntsService, IRecherche<Emprunts> rechercheEmprunt, IRecherche<Retours> rechercheRetour)
        {
            _unitOfWork = unitOfWork;
            _empruntsService = empruntsService;
            _livresService = livresService;
            _rechercheEmprunt = rechercheEmprunt;
            _rechercheRetour = rechercheRetour;
        }

        public async Task AddAsync(Retours retours)
        {
            ValidationService.VerifierNull(retours, nameof(retours), "Le retour");

            ValidationService.VerifierDate(retours.DateRetour, "de retour");

            var emprunt = await _empruntsService.GetByIdAsync(retours.IDEmprunt);
            ValidationService.EnregistrementNonTrouve(emprunt, "Emprunts", retours.IDEmprunt);

            var retour = new Retours
            {
                IDEmprunt = retours.IDEmprunt,
                DateRetour = retours.DateRetour
            };
            await _livresService.MettreAJourStock(emprunt.IDLivre, 1);
            await _unitOfWork.Retours.AddAsync(retours);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.Retours.DeleteAsync(id);
        }

        public async Task<IEnumerable<Retours>> GetAllAsync()
        {
            return await _unitOfWork.Retours.GetAllAsync();
        }

        public async Task<Retours> GetByIdAsync(Guid id)
        {
            var retours = await _unitOfWork.Retours.GetByIdAsync(id);
            ValidationService.EnregistrementNonTrouve(retours, "Retours", id);
            return retours;
        }
        public async Task UpdateAsync(Retours retours)
        {
            ValidationService.VerifierDate(retours.DateRetour, "de retour");

            var retour = await _unitOfWork.Retours.GetByIdAsync(retours.ID);
            ValidationService.EnregistrementNonTrouve(retour, "Retours", retours.ID);

           // retour.DateRetour = nouvelleDateRetour;

            await _unitOfWork.Retours.UpdateAsync(retour);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<RetourViewModel>> ObtenirEmpruntsActif(Guid? usagerId = null)
        {
            try
            {
                var empruntsActifsQuery = _rechercheEmprunt.GetAll()
                    .Include(e => e.Livre)
                    .Include(e => e.Usager)
                    .Where(e => e.Retours == null);


                if (usagerId.HasValue)
                {
                    empruntsActifsQuery = empruntsActifsQuery.Where(e => e.IDUsager == usagerId.Value);
                }

                var empruntsActifs = await empruntsActifsQuery.ToListAsync();

                var viewModel = empruntsActifs.Select(e => new RetourViewModel
                {
                    IDEmprunt = e.ID,
                    LivreTitre = e.Livre.Titre,
                    UsagerNom = e.Usager.Nom + " " + e.Usager.Prenoms,
                    DateEmprunt = e.DateDebut,
                    DatePrevu = e.DateRetourPrevue
                }).ToList();

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Emprunts"), ex);
            }
        }

        public async Task<IEnumerable<RetourViewModel>> ObtenirEmpruntsInActif()
        {
            try
            {
                var retours = await _rechercheRetour.GetAll()
                    .Include(r => r.Emprunt)
                    .ThenInclude(e => e.Livre)
                    .Include(r => r.Emprunt.Usager)
                    .Where(r => r.DateRetour != null)
                    .ToListAsync();

                var viewModel = retours.Select(retour => new RetourViewModel
                {
                    IDEmprunt = retour.IDEmprunt,
                    LivreTitre = retour.Emprunt.Livre.Titre,
                    UsagerNom = retour.Emprunt.Usager.Nom + " " + retour.Emprunt.Usager.Prenoms,
                    DateRetour = retour.DateRetour,
                    DateEmprunt = retour.Emprunt.DateDebut,
                    DatePrevu = retour.Emprunt.DateRetourPrevue
                }).ToList();

                return viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Emprunts"), ex);
            }
        }

        /// <summary>
        /// Filtre pour recherche des emprunts (par titre de livre ou nom usager)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RetourViewModel> FiltrerEmpruntsParRecherche(IEnumerable<RetourViewModel> emprunts, string recherche)// 
        {
            try
            {
                if (!string.IsNullOrEmpty(recherche))
                {
                    return emprunts.Where(e =>
                        e.LivreTitre.Contains(recherche, StringComparison.OrdinalIgnoreCase) ||
                        e.UsagerNom.Contains(recherche, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                return emprunts;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Emprunts"), ex);
            }
        }



    }
}
