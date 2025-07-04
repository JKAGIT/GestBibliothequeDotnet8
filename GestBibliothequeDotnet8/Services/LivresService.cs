using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Services
{
    public class LivresService : ILivres
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityValidationService<Livres> _entityValidationService;
        private readonly IRecherche<Livres> _rechercheLivre;
        private readonly IRecherche<Emprunts> _rechercheEmprunt;

        public LivresService(IUnitOfWork unitOfWork, IEntityValidationService<Livres> entityValidationService, IRecherche<Livres> recherche, IRecherche<Emprunts> rechercheEmprunt)
        {
            _unitOfWork = unitOfWork;
            _entityValidationService = entityValidationService;
            _rechercheLivre = recherche;
            _rechercheEmprunt = rechercheEmprunt;
        }

        public async Task AddAsync(Livres livre)
        {
            ValidationService.VerifierNull(livre, nameof(livre), "Le livre");

            if (await _entityValidationService.VerifierExistenceAsync(l => l.Titre == livre.Titre))
                throw new InvalidOperationException(string.Format(ErreurMessage.EntiteExisteDeja, "Un livre", livre.Titre));

            await _unitOfWork.Livres.AddAsync(livre);
            await _unitOfWork.CompleteAsync();
        } 

        public async Task UpdateAsync(Livres livre)
        {
            ValidationService.VerifierNull(livre, nameof(livre), "Le livre");

            var livreAModifier = await _unitOfWork.Livres.GetByIdAsync(livre.ID);
            ValidationService.EnregistrementNonTrouve(livreAModifier, "Livres", livre.ID);

            await _unitOfWork.Livres.UpdateAsync(livre);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid idLivre)
        {
            var empruntsActifs = await _rechercheEmprunt.FindAsync(e => e.IDLivre == idLivre && e.Retours == null);
            if (empruntsActifs.Any())
            {
                throw new InvalidOperationException(string.Format(ErreurMessage.ErreurSuppressionEntiteLiee, "un livre", "emprunts actifs"));
            }

            await _unitOfWork.Livres.DeleteAsync(idLivre);
            await _unitOfWork.CompleteAsync();
        }

        public async Task MettreAJourStock(Guid idLivre, int nouveauStock)
        {
            var livre = await _unitOfWork.Livres.GetByIdAsync(idLivre);
            ValidationService.EnregistrementNonTrouve(livre, "Livres", idLivre);

            livre.Stock += nouveauStock;
            await _unitOfWork.Livres.UpdateAsync(livre);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> EstDisponible(Guid idLivre)
        {
            var livre = await _unitOfWork.Livres.GetByIdAsync(idLivre);
            ValidationService.EnregistrementNonTrouve(livre, "Livres", idLivre);
            return livre != null && livre.Stock > 0;
        }

        public async Task<Livres> GetByIdAsync(Guid idLivre)
        {
            var livre = await _unitOfWork.Livres.GetByIdAsync(idLivre);
            ValidationService.EnregistrementNonTrouve(livre, "Livres", idLivre);
            return livre;
        }
        public async Task<IEnumerable<Livres>> ObtenirLivresParCategorie(Guid idCategorie)
        {
            try
            {
                return await _rechercheLivre.FindAsync(l => l.IDCategorie == idCategorie);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Livres"), ex);
            }
        }

        public async Task<IEnumerable<Livres>> ObtenirLivresEnStock()
        {
            try
            {
                return await _rechercheLivre.FindAsync(l => l.Stock > 0);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Livres"), ex);
            }
        }

        public async Task<IEnumerable<Livres>> GetAllAsync()  //lazy loading
        {
            try
            {
                return await _unitOfWork.Livres.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Livres"), ex);
            }
        }


        public async Task<IEnumerable<Livres>> ObtenirLivresAvecCategories(Guid? id = null)
        {
            try
            {
                IQueryable<Livres> query = _rechercheLivre.GetAll().Include(l => l.Categories);

                if (id.HasValue)
                {
                    query = query.Where(l => l.ID == id.Value);
                }

                return await query.ToListAsync();  
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Livres"), ex);
            }
        }

   }
}
