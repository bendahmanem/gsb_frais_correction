namespace gsb_frais_correction.Models
{
    public class LigneFraisForfait
    {
        public int FicheId { get; set; }
        public int TypeFraisId { get; set; }
        public int Quantite { get; set; }
        public decimal MontantUnitaire { get; set; }
        public decimal MontantTotal => Quantite * MontantUnitaire;
        public bool Accepte { get; set; }
        public string MotifRefus { get; set; }

        // Navigation properties
        public FicheFrais FicheFrais { get; set; }
        public TypeFrais TypeFrais { get; set; }

        public LigneFraisForfait()
        {
            Accepte = true;
            Quantite = 0;
        }
    }
}