using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface IEntityValidationService<T> where T : class
    {
        Task<bool> VerifierExistenceAsync(Expression<Func<T, bool>> predicate);
    }
}
