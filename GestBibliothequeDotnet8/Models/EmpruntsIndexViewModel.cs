namespace GestBibliothequeDotnet8.Models
{
    internal class EmpruntsIndexViewModel
    {
        public IEnumerable<RetourViewModel> EmpruntsActifs { get; set; }
        public IEnumerable<RetourViewModel> EmpruntsInactifs { get; set; }
    }
    public class EmpruntViewModel
    {
        public Guid ID { get; set; }
        public string LivreTitre { get; set; }
        public string UsagerNom { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateRetourPrevue { get; set; }
        public DateTime? DateRetour { get; set; }
    }
}