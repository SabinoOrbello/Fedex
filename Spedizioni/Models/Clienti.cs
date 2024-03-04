using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class Clienti
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il campo Tipo Cliente è obbligatorio.")]
        [RegularExpression("^(Privato|Azienda)$", ErrorMessage = "Il Tipo Cliente deve essere 'Privato' o 'Azienda'.")]
        public string TipoCliente { get; set; }


        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il Codice Fiscale deve essere di 16 caratteri.")]
        public string CodiceFiscale { get; set; }


        [StringLength(20, ErrorMessage = "La Partita IVA può avere al massimo 20 caratteri.")]
        public string PartitaIva { get; set; }
    }
}