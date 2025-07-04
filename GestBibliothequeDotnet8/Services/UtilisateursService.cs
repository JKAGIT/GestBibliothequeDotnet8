using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Utilitaires;

namespace GestBibliothequeDotnet8.Services
{
    public class UtilisateursService : IUtilisateurs
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityValidationService<Utilisateurs> _entityValidationService;

        public UtilisateursService(IUnitOfWork unitOfWork, IEntityValidationService<Utilisateurs> entityValidationService)
        {
            _unitOfWork = unitOfWork;
            _entityValidationService = entityValidationService;
        }
        public async Task AddAsync(Utilisateurs utilisateur)
        {
            ValidationService.VerifierNull(utilisateur, nameof(utilisateur), "L'utilisateur");

            if (await _entityValidationService.VerifierExistenceAsync(u => u.Matricule == utilisateur.Matricule))
                throw new InvalidOperationException(string.Format(ErreurMessage.EntiteExisteDeja, "Un utilisateur", utilisateur.Matricule));

            if (await _entityValidationService.VerifierExistenceAsync(u => u.Courriel == utilisateur.Courriel))
                throw new InvalidOperationException(string.Format(ErreurMessage.EntiteExisteDeja, "Un utilisateur", utilisateur.Courriel));

            await _unitOfWork.Utilisateurs.AddAsync(utilisateur);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.Utilisateurs.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Utilisateurs>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.Utilisateurs.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, "Utilisateurs"), ex);
            }
        }

        public async Task<Utilisateurs> GetByIdAsync(Guid id)
        {
            var utilisateur = await _unitOfWork.Utilisateurs.GetByIdAsync(id);
            ValidationService.EnregistrementNonTrouve(utilisateur, "Utilisateurs", id);
            return utilisateur;
        }

        public async Task UpdateAsync(Utilisateurs utilisateur)
        {
            ValidationService.VerifierNull(utilisateur, nameof(utilisateur), "L'utilisateur");

            var utilisateurAModifier = await _unitOfWork.Utilisateurs.GetByIdAsync(utilisateur.ID);
            ValidationService.EnregistrementNonTrouve(utilisateurAModifier, "Utilisateurs", utilisateur.ID);

            await _unitOfWork.Utilisateurs.UpdateAsync(utilisateur);
            await _unitOfWork.CompleteAsync();
        }
    }
}
