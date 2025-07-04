
using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.EntityFrameworkCore;

namespace GestBibliothequeDotnet8.Services
{
    public class CategoriesService : ICategories
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityValidationService<Categories> _entityValidationService;
        private readonly IRecherche<Categories> _recherche;

        public CategoriesService(IUnitOfWork unitOfWork, IEntityValidationService<Categories> entityValidationService, IRecherche<Categories> recherche)
        {
            _unitOfWork = unitOfWork;
            _entityValidationService = entityValidationService;
            _recherche = recherche;
        }


        public async Task AddAsync(Categories categorie)
        {
            ValidationService.VerifierNull(categorie, nameof(categorie), "La catégorie");
            if (await _entityValidationService.VerifierExistenceAsync(c => c.Code == categorie.Code))
                throw new InvalidOperationException(string.Format(ErreurMessage.EntiteExisteDeja, "Une catégorie", categorie.Code));

            await _unitOfWork.Categories.AddAsync(categorie);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Categories categorie)
        {
            ValidationService.VerifierNull(categorie, nameof(categorie), "La catégorie");
            var categorieAModifier = await _unitOfWork.Categories.GetByIdAsync(categorie.ID);
            ValidationService.EnregistrementNonTrouve(categorieAModifier, "Categories", categorie.ID);

            await _unitOfWork.Categories.UpdateAsync(categorie);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid idCategorie)
        {
            var categorieASupprimer = await _recherche.GetAll()
                                                .Include(c => c.Livres)
                                                .FirstOrDefaultAsync(c => c.ID == idCategorie);

            ValidationService.EnregistrementNonTrouve(categorieASupprimer, "Categories", idCategorie);

            if (categorieASupprimer.Livres != null && categorieASupprimer.Livres.Any())
                throw new InvalidOperationException(string.Format(ErreurMessage.ErreurSuppressionEntiteLiee, "une catégorie", "livres"));

            await _unitOfWork.Categories.DeleteAsync(idCategorie);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Categories>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.Categories.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Categories"), ex);
            }
        }
        /**************/
        public async Task<PaginatedResult<Categories>> GetPagedAsync(int pageNumber, int pageSize)
        {           
            var query = _recherche.GetAll(); 

            var totalItems = await query.CountAsync(); 
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Categories>
            {
                Data = data,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /******************/

        public async Task<Categories> GetByIdAsync(Guid idCategorie)
        {
            var categorie = await _unitOfWork.Categories.GetByIdAsync(idCategorie);
            ValidationService.EnregistrementNonTrouve(categorie, "Categories", idCategorie);
            return categorie;
        }

        public async Task<Categories> ObtenirCategorieParCode(string code)
        {
            return await _recherche.GetAll().FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<bool> VerifierExistenceDansLivres(Guid categorieId)
        {
            return await _recherche.GetAll()
                     .Where(c => c.ID == categorieId)
                    .SelectMany(c => c.Livres)
                    .AnyAsync();
        }     

    }
}
