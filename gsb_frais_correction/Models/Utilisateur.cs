using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsb_frais_correction.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Secteur { get; set; }
        public DateTime DateCreation { get; set; }
        public bool Actif { get; set; }

        // Calculated properties
        public string NomComplet => $"{Prenom} {Nom}";
        public bool EstAdministrateur => Role == "ADMINISTRATEUR";
        public bool EstVisiteur => Role == "VISITEUR";

        public bool EstComptable => Role == "COMPTABLE";

        // Constructor
        public Utilisateur()
        {
            DateCreation = DateTime.Now;
            Actif = true;
            Secteur = "Non défini";
        }
    }
}
