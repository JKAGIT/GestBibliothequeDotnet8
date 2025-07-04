namespace GestBibliothequeDotnet8.Models
{
   
    public class RetourViewModel
    {
        public Guid IDEmprunt { get; set; }  
        public string LivreTitre { get; set; }  
        public string UsagerNom { get; set; }
        public DateTime DateEmprunt { get; set; }
        public DateTime DatePrevu { get; set; }
        public DateTime DateRetour { get; set; }
    }

}
