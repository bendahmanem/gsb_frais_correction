using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsb_frais_correction.Models
{
    public class FicheFrais
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public int Mois { get; set; }
        public int Annee { get; set; }
        public string Etat { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public int NbJustificatifs { get; set; }
        public decimal MontantValide { get; set; }
        public DateTime? DateValidation { get; set; }
        public string MotifRefus { get; set; }

        // Navigation properties 
        public Utilisateur Utilisateur { get; set; }
        public List<LigneFraisForfait> LignesFraisForfait { get; set; }

        public List<LigneFraisHF> LignesFraisHF { get; set; }

        // Calculated properties
        // Propriétés calculées
        public string PeriodeLibelle => $"{GetNomMois(Mois)} {Annee}";
        public decimal MontantTotal => MontantForfait + MontantHorsForfait;
        public decimal MontantForfait { get; set; }
        public decimal MontantHorsForfait { get; set; }
        public bool PeutEtreModifiee => Etat == "EN_COURS";
        public bool EstValidee => Etat == "VALIDEE";

        public FicheFrais()
        {
            DateCreation = DateTime.Now;
            DateModification = DateTime.Now;
            Etat = "EN_COURS";
            LignesFraisForfait = new List<LigneFraisForfait>();
            LignesFraisHF = new List<LigneFraisHF>();
        }

        private string GetNomMois(int mois)
        {
            string[] noms = { "", "Janvier", "Février", "Mars", "Avril", "Mai", "Juin",
                            "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };
            return mois >= 1 && mois <= 12 ? noms[mois] : "Inconnu";
        }


    }
    }
