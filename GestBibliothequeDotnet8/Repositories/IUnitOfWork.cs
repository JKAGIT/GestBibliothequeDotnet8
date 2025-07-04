using GestBibliothequeDotnet8.Models;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Livres> Livres { get; }
        IGenericRepository<Categories> Categories { get; }
        IGenericRepository<Utilisateurs> Utilisateurs { get; }
        IGenericRepository<Usagers> Usagers { get; }
        IGenericRepository<Emprunts> Emprunts { get; }
        IGenericRepository<Retours> Retours { get; }
        IGenericRepository<Reservations> Reservations { get; }
        Task<int> CompleteAsync();
    }
}
