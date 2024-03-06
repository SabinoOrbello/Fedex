using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Spedizioni.Models
{
    public class VerificaStatoSpedizioni
    {
        public TipoCliente TipoCliente { get; set; } // Può essere "Privato" o "Azienda"
        public string CodiceFiscale { get; set; }
        public string PartitaIva { get; set; }
        public int SpedizioneId { get; set; }
    }
}