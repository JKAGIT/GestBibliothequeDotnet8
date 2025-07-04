using System.ComponentModel.DataAnnotations;

namespace GestBibliothequeDotnet8.Models
{
    public class Categories
    {
        [Key] 
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Le Code est obligatoire.")]
        [StringLength(4, ErrorMessage = "Le Code doit être exactement de 4 caractères.")]
        [RegularExpression(@"^[a-zA-Z0-9]{4}$", ErrorMessage = "Le Code doit contenir uniquement 4 caractères alphanumériques.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Le Libellé est obligatoire.")]
        public string Libelle { get; set; }
        public ICollection<Livres> Livres { get; set; } = new List<Livres>();

    }
}
