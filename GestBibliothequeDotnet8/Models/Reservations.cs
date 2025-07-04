using System.ComponentModel.DataAnnotations;

namespace GestBibliothequeDotnet8.Models
{
    public class Reservations
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateRetourEstimee { get; set; }

        public bool Annuler { get; set; } 

        public Guid IDUsager { get; set; }
        public Usagers? Usager { get; set; }

        public Guid IDLivre { get; set; }
        public Livres? Livre { get; set; }
        public virtual Emprunts? Emprunt { get; set; }


    }
}
