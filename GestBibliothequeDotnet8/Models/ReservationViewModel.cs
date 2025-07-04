using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestBibliothequeDotnet8.Models
{
    public class ReservationViewModel
    {
        public Guid IdEmprunt { get; set; }
        public Guid IdLivre { get; set; }
        public Guid IdUsager { get; set; }
        public Guid IdReservation { get; set; }
        public string? LivreTitre { get; set; }
        public string? UsagerNom { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DatePrevue { get; set; }
        public bool Annuler { get; set; }
        public bool EstDisponible { get; set; }

        public IEnumerable<SelectListItem>? Usagers { get; set; }
        public IEnumerable<SelectListItem>? Livres { get; set; }

    }
}
