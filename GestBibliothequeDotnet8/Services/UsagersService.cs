using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;

namespace GestBibliothequeDotnet8.Services
{

    public class UsagersService : IUsagers
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityValidationService<Usagers> _entityValidationService;
        private readonly IRecherche<Emprunts> _rechercheEmprunt;

        public UsagersService(IUnitOfWork unitOfWork, IEntityValidationService<Usagers> entityValidationService, IRecherche<Emprunts> recherche)
        {
            _unitOfWork = unitOfWork;
            _entityValidationService = entityValidationService;
            _rechercheEmprunt = recherche;
        }

        public async Task AddAsync(Usagers usager)
        {
            ValidationService.VerifierNull(usager, nameof(usager), "L'usager");

            if (await _entityValidationService.VerifierExistenceAsync(u => u.Courriel == usager.Courriel))
                throw new InvalidOperationException(string.Format(ErreurMessage.EntiteExisteDeja, "Un usager", usager.Courriel));

            await _unitOfWork.Usagers.AddAsync(usager);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid idUsager)
        {
            var empruntsActifs = await _rechercheEmprunt.FindAsync(e => e.IDUsager == idUsager && e.Retours == null);

            if (empruntsActifs.Any())
                throw new InvalidOperationException(string.Format(ErreurMessage.ErreurSuppressionEntiteLiee, "un usager", "emprunts actifs"));

            await _unitOfWork.Usagers.DeleteAsync(idUsager);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<IEnumerable<Usagers>> GetAllAsync()
        {
            return await _unitOfWork.Usagers.GetAllAsync();
        }

        public async Task<Usagers> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Usagers.GetByIdAsync(id);
        }
        public async Task UpdateAsync(Usagers usager)
        {
            ValidationService.VerifierNull(usager, nameof(usager), "L'usager");

            var usagerAModifier = await _unitOfWork.Usagers.GetByIdAsync(usager.ID);
            ValidationService.EnregistrementNonTrouve(usagerAModifier, "Usagers", usager.ID);

            await _unitOfWork.Usagers.UpdateAsync(usager);
            await _unitOfWork.CompleteAsync();
        }
    }


}
