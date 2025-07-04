using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Utilitaires;
using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface ICategories: IGenericRepository<Categories>
    {
        Task<Categories> ObtenirCategorieParCode(string code);
        Task<bool> VerifierExistenceDansLivres(Guid categorieId);
        Task<PaginatedResult<Categories>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
