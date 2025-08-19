namespace gsb_frais_correction.Models
{
    public class Justificatif
    {
        public int Id { get; set; }
        public string NomFichier { get; set; }
        public string CheminFichier { get; set; }
        public long TailleFichier { get; set; }
        public string TypeMime { get; set; }
        public DateTime DateUpload { get; set; }
        public string Description { get; set; }

        public Justificatif()
        {
            DateUpload = DateTime.Now;
            TypeMime = "application/pdf";
        }
    }
}