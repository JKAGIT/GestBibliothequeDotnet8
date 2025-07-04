using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface IRecherche<T> where T : class
    {
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll(); // possibilité d'ajouter des filtres --Lazy Loading
    }
}
