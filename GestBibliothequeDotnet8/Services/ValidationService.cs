using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Utilitaires;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestBibliothequeDotnet8.Services
{
    public static class ValidationService
    {
        public static void VerifierNull<T>(T valeur, string nomArgument, string paramArgument)
        {
            if (valeur == null)
            {
                throw new ArgumentNullException(nomArgument, string.Format(ErreurMessage.ValeurNulle, paramArgument));
            }
        }

        public static void EnregistrementNonTrouve<T>(T entite, string nomEntite, Guid identifiant)
        {
            if (entite == null)
            {
                throw new KeyNotFoundException(string.Format(ErreurMessage.EnregistrementNonTrouve, nomEntite, identifiant));                
            }
        }

        public static void VerifierDate(DateTime? date, string typeDate)
        {
            if (date == null || date == default)
            {
                throw new KeyNotFoundException(string.Format(ErreurMessage.DateValide, typeDate));
            }
        }

    }
}