using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsb_frais_correction.Models
{
    public class TypeFrais
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Libelle { get; set; }
        public bool Forfaitaire { get; set; }
        public decimal Tarif { get; set; }
        public bool Actif { get; set; }
        public DateTime DateCreation { get; set; }

        public TypeFrais()
        {
            DateCreation = DateTime.Now;
            Actif = true;
            Forfaitaire = true;
        }
    }
}
