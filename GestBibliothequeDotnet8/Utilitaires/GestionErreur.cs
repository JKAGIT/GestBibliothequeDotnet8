using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestBibliothequeDotnet8.Utilitaires
{
    public static class GestionErreurs
    {
        public static void GererErreur(Exception ex, Controller controller)
        {
            string messageErreur = ex is ArgumentNullException || ex is InvalidOperationException || ex is KeyNotFoundException
                ? ex.Message
                : $"Une erreur inattendue est survenue. Détails : {ex.Message}";

            if (!string.IsNullOrWhiteSpace(messageErreur))
            {
                // Singleton
                //var logger = LoggerSingleton.Instance;  // Récupère l'instance de LoggerSingleton
                //logger.LogError(messageErreur);  // Log l'erreur dans le fichier log

                // methode statique
                Logger.LogError(messageErreur);
            }
            controller.ModelState.AddModelError(string.Empty, messageErreur);
        }
    }
}
