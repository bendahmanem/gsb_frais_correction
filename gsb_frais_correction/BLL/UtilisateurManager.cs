using gsb_frais_correction.DAL;
using gsb_frais_correction.Models;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gsb_frais_correction.BLL
{
    public class UtilisateurManager
    {
        private readonly UtilisateurDAO _utilisateurDAO;
        private static Utilisateur _utilisateurConnecte;

        public UtilisateurManager()
        {
            _utilisateurDAO = new UtilisateurDAO();
        }

        public static Utilisateur UtilisateurConnecte
        {
            get => _utilisateurConnecte;
            private set => _utilisateurConnecte = value;
        }

        public bool Connecter(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Le login et le mot de passe sont obligatoires.");
            }
            var utilisateur = _utilisateurDAO.Authentifier(login, password);
            if (utilisateur != null)
            {
                UtilisateurConnecte = utilisateur;
                return true;
            }

            return false;
        }

        public void Deconnecter()
        {
            UtilisateurConnecte = null;
        }

        public List<Utilisateur> GetTousLesUtilisateurs()
        {
            return _utilisateurDAO.GetTousLesUtilisateurs();
        }

        public List<Utilisateur> GetVisiteurs()
        {
            return _utilisateurDAO.GetVisiteurs();
        }

        public bool CreerUtilisateur(Utilisateur utilisateur)
        {
            ValiderUtilisateur(utilisateur);
            return _utilisateurDAO.CreerUtilisateur(utilisateur);
        }

        public bool ModifierUtilisateur(Utilisateur utilisateur)
        {
            ValiderUtilisateur(utilisateur, false);
            return _utilisateurDAO.ModifierUtilisateur(utilisateur);
        }

        public bool SupprimerUtilisateur(int id)
        {
            if (id == UtilisateurConnecte?.Id)
            {
                throw new InvalidOperationException("Impossible de supprimer l'utilisateur connecté.");
            }

            return _utilisateurDAO.SupprimerUtilisateur(id);
        }

        private void ValiderUtilisateur(Utilisateur utilisateur, bool nouveauUtilisateur = true)
        {
            if (string.IsNullOrWhiteSpace(utilisateur.Login))
                throw new ArgumentException("Le login est obligatoire.");

            if (string.IsNullOrWhiteSpace(utilisateur.Nom))
                throw new ArgumentException("Le nom est obligatoire.");

            if (string.IsNullOrWhiteSpace(utilisateur.Prenom))
                throw new ArgumentException("Le prénom est obligatoire.");

            if (string.IsNullOrWhiteSpace(utilisateur.Email))
                throw new ArgumentException("L'email est obligatoire.");

            if (!Regex.IsMatch(utilisateur.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("L'email n'est pas valide.");

            if (nouveauUtilisateur && string.IsNullOrWhiteSpace(utilisateur.Password))
                throw new ArgumentException("Le mot de passe est obligatoire.");

            if (!string.IsNullOrEmpty(utilisateur.Password) && utilisateur.Password.Length < 6)
                throw new ArgumentException("Le mot de passe doit contenir au moins 6 caractères.");

            var rolesValides = new[] { "ADMINISTRATEUR", "VISITEUR", "COMPTABLE" };
            if (!Array.Exists(rolesValides, r => r == utilisateur.Role))
                throw new ArgumentException("Le rôle sélectionné n'est pas valide.");
        }
    }
}
