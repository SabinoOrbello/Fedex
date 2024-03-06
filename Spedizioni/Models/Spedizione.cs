using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class Spedizione
    {
        public int SpedizioneId { get; set; }

        [Required(ErrorMessage = "Il campo Cliente è obbligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Il campo Data Spedizione è obbligatorio.")]
        public DateTime DataSpedizione { get; set; }

        [Required(ErrorMessage = "Il campo Peso è obbligatorio.")]
        public float Peso { get; set; }

        [Required(ErrorMessage = "Il campo Città Destinataria è obbligatorio.")]
        public string CittaDestinataria { get; set; }

        [Required(ErrorMessage = "Il campo Indirizzo Destinatario è obbligatorio.")]
        public string IndirizzoDestinatario { get; set; }

        [Required(ErrorMessage = "Il campo Nominativo Destinatario è obbligatorio.")]
        public string NominativoDestinatario { get; set; }

        [Required(ErrorMessage = "Il campo Costo Spedizione è obbligatorio.")]
        public decimal CostoSpedizione { get; set; }

        [Required(ErrorMessage = "Il campo Data Consegna Prevista è obbligatorio.")]
        public DateTime DataConsegnaPrevista { get; set; }

    }
}