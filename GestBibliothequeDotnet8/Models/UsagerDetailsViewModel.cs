namespace GestBibliothequeDotnet8.Models
{
    public class UsagerDetailsViewModel
    {  
        public Guid UsagerId { get; set; }
        public string Nom { get; set; }
        public string Prenoms { get; set; }
        public string Courriel { get; set; }
        public string Telephone { get; set; }

        public IEnumerable<RetourViewModel> Emprunts { get; set; }
    }
}
