using gsb_frais_correction.DAL;
using gsb_frais_correction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsb_frais_correction.BLL
{
    public class FicheFraisManager
    {
        private readonly FicheFraisDAO _ficheFraisDAO;
        private readonly LigneFraisDAO _ligneFraisDAO;

        public FicheFraisManager()
        {
            _ficheFraisDAO = new FicheFraisDAO();
            _ligneFraisDAO = new LigneFraisDAO();
        }

        public List<FicheFrais> GetFichesByUtilisateur(int utilisateurId)
        {
            return _ficheFraisDAO.GetFichesByUtilisateur(utilisateurId);
        }

        public FicheFrais GetFicheCourante(int utilisateurId)
        {
            return _ficheFraisDAO.GetOuCreerFicheCourante(utilisateurId);
        }

        public FicheFrais GetFicheComplete(int ficheId)
        {
            var fiche = _ficheFraisDAO.GetFiche(ficheId);
            if (fiche != null)
            {
                fiche.LignesFraisForfait = _ligneFraisDAO.GetLignesForfait(ficheId);
                fiche.LignesFraisHF = _ligneFraisDAO.GetLignesHorsForfait(ficheId);
            }
            return fiche;
        }

        public bool ModifierFraisForfaitaires(int ficheId, Dictionary<int, int> quantites)
        {
            try
            {
                foreach (var kvp in quantites)
                {
                    if (kvp.Value < 0)
                        throw new ArgumentException($"La quantité ne peut pas être négative.");

                    _ligneFraisDAO.ModifierQuantiteForfait(ficheId, kvp.Key, kvp.Value);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la modification des frais forfaitaires : {ex.Message}");
            }
        }

        public bool AjouterFraisHorsForfait(LigneFraisHF ligne)
        {
            ValiderLigneFraisHF(ligne);
            return _ligneFraisDAO.AjouterLigneHorsForfait(ligne);
        }

        public bool SupprimerFraisHorsForfait(int ligneId)
        {
            return _ligneFraisDAO.SupprimerLigneHorsForfait(ligneId);
        }

        public bool ValiderFiche(int ficheId)
        {
            return _ficheFraisDAO.ModifierEtatFiche(ficheId, "VALIDEE");
        }

        public bool RefuserFiche(int ficheId, string motif)
        {
            if (string.IsNullOrWhiteSpace(motif))
                throw new ArgumentException("Un motif de refus est obligatoire.");

            return _ficheFraisDAO.ModifierEtatFiche(ficheId, "REFUSEE", motif);
        }

        public bool RefuserPartiellement(int ficheId, List<int> lignesRefusees, string motif)
        {
            if (string.IsNullOrWhiteSpace(motif))
                throw new ArgumentException("Un motif de refus est obligatoire.");

            try
            {
                foreach (var ligneId in lignesRefusees)
                {
                    _ligneFraisDAO.RefuserLigne(ligneId, motif);
                }

                return _ficheFraisDAO.ModifierEtatFiche(ficheId, "REFUS_PARTIEL", motif);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du refus partiel : {ex.Message}");
            }
        }

        public bool PeutModifierFiche(FicheFrais fiche)
        {
            return fiche?.Etat == "EN_COURS" && EstDansPeriodeSaisie(fiche.Mois, fiche.Annee);
        }

        private bool EstDansPeriodeSaisie(int mois, int annee)
        {
            var maintenant = DateTime.Now;
            var dateFiche = new DateTime(annee, mois, 1);

            // Règle des 10 jours
            if (maintenant.Day <= 10)
            {
                var moisPrecedent = maintenant.AddMonths(-1);
                return dateFiche.Month == moisPrecedent.Month && dateFiche.Year == moisPrecedent.Year;
            }

            return dateFiche.Month == maintenant.Month && dateFiche.Year == maintenant.Year;
        }

        private void ValiderLigneFraisHF(LigneFraisHF ligne)
        {
            if (string.IsNullOrWhiteSpace(ligne.Libelle))
                throw new ArgumentException("Le libellé est obligatoire.");

            if (ligne.Montant <= 0)
                throw new ArgumentException("Le montant doit être positif.");

            if (ligne.DateFrais > DateTime.Today)
                throw new ArgumentException("La date ne peut pas être dans le futur.");

            if (ligne.DateFrais < DateTime.Today.AddMonths(-6))
                throw new ArgumentException("La date ne peut pas être antérieure à 6 mois.");
        }
    }
}
