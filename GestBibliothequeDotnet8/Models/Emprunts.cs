using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestBibliothequeDotnet8.Models
{
    public class Emprunts
    {
        [Key]
        public Guid ID { get; set; }

        public Guid? IDReservation { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire.")]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de retour prévue est obligatoire.")]
        public DateTime DateRetourPrevue { get; set; }

        [ForeignKey("Categorie")]
        public Guid IDUsager { get; set; }
        public Usagers? Usager { get; set; } 

        [ForeignKey("Categorie")]
        public Guid IDLivre { get; set; }
        public Livres? Livre { get; set; }
        public virtual Retours? Retours { get; set; }
        public virtual Reservations? Reservation { get; set; }
    }
}
