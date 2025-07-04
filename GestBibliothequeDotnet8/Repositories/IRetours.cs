using GestBibliothequeDotnet8.Models;

namespace GestBibliothequeDotnet8.Repositories
{
    public interface IRetours: IGenericRepository<Retours>
    {
        Task<IEnumerable<RetourViewModel>> ObtenirEmpruntsActif(Guid? usagerId = null);
        Task<IEnumerable<RetourViewModel>> ObtenirEmpruntsInActif();
        IEnumerable<RetourViewModel> FiltrerEmpruntsParRecherche(IEnumerable<RetourViewModel> emprunts, string recherche);
    }
}
