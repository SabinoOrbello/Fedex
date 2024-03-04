using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class AggiornamentoSpedizione
    {
        public int AggiornamentoId { get; set; }

        [Required(ErrorMessage = "Il campo Spedizione è obbligatorio.")]
        public int SpedizioneId { get; set; }

        [Required(ErrorMessage = "Il campo Stato è obbligatorio.")]
        public string Stato { get; set; }

        [Required(ErrorMessage = "Il campo Data e Ora Aggiornamento è obbligatorio.")]
        public DateTime DataOraAggiornamento { get; set; }
    }
}