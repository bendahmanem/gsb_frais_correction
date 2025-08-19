namespace gsb_frais_correction.Models
{
    public class LigneFraisHF
    {
        public int Id { get; set; }
        public int FicheId { get; set; }
        public string Libelle { get; set; }
        public DateTime DateFrais { get; set; }
        public decimal Montant { get; set; }
        public bool Accepte { get; set; }
        public string MotifRefus { get; set; }
        public int? JustificatifId { get; set; }

        // Navigation properties
        public FicheFrais FicheFrais { get; set; }
        public Justificatif Justificatif { get; set; }

        public LigneFraisHF()
        {
            Accepte = true;
            DateFrais = DateTime.Today;
        }
    }
}