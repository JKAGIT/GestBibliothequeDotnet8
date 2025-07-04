using GestBibliothequeDotnet8.Models;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface IEmprunts:IGenericRepository<Emprunts>
    {
        Task<IEnumerable<Emprunts>> ObtenirEmpruntParUsager(Guid idUsager);
        Task AjouterEmpruntReservation(Guid idReservation);
    }
}
