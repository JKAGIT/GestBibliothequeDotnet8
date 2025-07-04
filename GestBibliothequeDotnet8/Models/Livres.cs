using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestBibliothequeDotnet8.Models
{
    public class Livres
    {
        [Key]
        public Guid ID { get; set; }
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Titre { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public string Auteur { get; set; }

        [Required(ErrorMessage = "Le Nombre est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez saisir un nombre positif.")]

        public int Nombre { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Le stock doit être supérieur ou égal à 0.")]
        public int Stock { get; set; }

        [ForeignKey("Categorie")] 
        public Guid IDCategorie { get; set; }
        public Categories? Categories { get; set; }

        public ICollection<Emprunts> Emprunts { get; set; } = new List<Emprunts>();
        public ICollection<Reservations> Reservations { get; set; } = new List<Reservations>();  

    }
}
