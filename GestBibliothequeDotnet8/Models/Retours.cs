using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestBibliothequeDotnet8.Models
{
    public class Retours
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [ForeignKey("Emprunt")]
        public Guid IDEmprunt { get; set; }

        public Emprunts? Emprunt { get; set; }

        [Required]
        public DateTime DateRetour { get; set; }
    }
}
