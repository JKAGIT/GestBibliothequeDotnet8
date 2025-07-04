using GestBibliothequeDotnet8.Repositories;
using GestBibliothequeDotnet8.Donnee;
using Microsoft.EntityFrameworkCore;
using GestBibliothequeDotnet8.Models;
using GestBibliothequeDotnet8.Utilitaires;

namespace GestBibliothequeDotnet8.Services
{
    public class GenerateurMatriculeUnique
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecherche<Utilisateurs> _recherche;

        public GenerateurMatriculeUnique(IUnitOfWork unitOfWork, IRecherche<Utilisateurs> recherche)
        {
            _unitOfWork = unitOfWork;
            _recherche = recherche;
        }

        public async Task<string> GenererMatriculeUnique()
        {
            string matricule;
            bool matriculeExiste;
            try
            {
                do
                {
                    matricule = GenererMatriculeAleatoire();
                    var result = await _recherche.FindAsync(u => u.Matricule == matricule);
                    matriculeExiste = result.Any();
                }
                while (matriculeExiste);

                return matricule;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErreurMessage.ErreurRecherche, ""), ex);
            }
        }

        private string GenererMatriculeAleatoire()
        {
            var prefixe = "MATR-";
            var randomNumber = new Random().Next(10000, 100000);
            var matricule = $"{prefixe}{randomNumber}";

            return matricule;
        }

    }
}

