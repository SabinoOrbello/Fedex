using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class StatoSpedizioniController : Controller
    {
        // GET: StatoSpedizioni
        string connectionString = ConfigurationManager.ConnectionStrings["Fedex"].ToString();

        public ActionResult ListaStatoSpedizioni()
        {
            List<AggiornamentoSpedizione> listaStatoSpedizioni = GetListaStatoSpedizioniFromDatabase();

            return View(listaStatoSpedizioni);
        }

        private List<AggiornamentoSpedizione> GetListaStatoSpedizioniFromDatabase()
        {
            List<AggiornamentoSpedizione> listaStatoSpedizioni = new List<AggiornamentoSpedizione>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM AggiornamentiSpedizioni", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AggiornamentoSpedizione Statospedizioni = new AggiornamentoSpedizione
                            {
                                AggiornamentoId = (int)reader["AggiornamentoId"],
                                SpedizioneId = (int)reader["SpedizioneId"],
                                Stato = reader["Stato"].ToString(),
                                Luogo = reader["Luogo"].ToString(),
                                Descrizione = reader["Descrizione"].ToString(),
                                DataOraAggiornamento = (DateTime)reader["DataOraAggiornamento"]

                            };

                            listaStatoSpedizioni.Add(Statospedizioni);
                        }
                    }
                }
            }

            return listaStatoSpedizioni;
        }

        public ActionResult VerificaStatoSpedizione()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerificaStatoSpedizione(VerificaStatoSpedizioni model)
        {
            switch (model.TipoCliente)
            {
                case TipoCliente.Privato:
                    if (VerificaAssociazioneClienteSpedizione(model.CodiceFiscale, null, model.SpedizioneId))
                    {
                        // Ottieni gli aggiornamenti della spedizione dal database
                        List<AggiornamentoSpedizione> aggiornamenti = GetStatiSpedizioneFromDatabase(model.SpedizioneId);
                        // Restituisci la vista con gli aggiornamenti
                        return View("AggiornamentiSpedizione", aggiornamenti);
                    }
                    break;

                case TipoCliente.Azienda:
                    if (VerificaAssociazioneClienteSpedizione(null, model.PartitaIva, model.SpedizioneId))
                    {
                        // Ottieni gli aggiornamenti della spedizione dal database
                        List<AggiornamentoSpedizione> aggiornamenti = GetStatiSpedizioneFromDatabase(model.SpedizioneId);
                        // Restituisci la vista con gli aggiornamenti
                        return View("AggiornamentiSpedizione", aggiornamenti);
                    }
                    break;
            }

            // Se l'associazione non è confermata, restituisci una vista indicando che non sono disponibili informazioni sulla spedizione
            return View("NessunaInformazioneSpedizione");
        }



        private List<AggiornamentoSpedizione> GetStatiSpedizioneFromDatabase(int spedizioneId)
        {
            List<AggiornamentoSpedizione> statiSpedizione = new List<AggiornamentoSpedizione>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM AggiornamentiSpedizioni WHERE SpedizioneId = @SpedizioneId ORDER BY DataOraAggiornamento DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SpedizioneId", spedizioneId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AggiornamentoSpedizione stato = new AggiornamentoSpedizione
                            {

                                SpedizioneId = (int)reader["SpedizioneId"],
                                Stato = reader["Stato"].ToString(),
                                Luogo = reader["Luogo"].ToString(),
                                Descrizione = reader["Descrizione"].ToString(),
                                DataOraAggiornamento = (DateTime)reader["DataOraAggiornamento"]
                            };

                            statiSpedizione.Add(stato);
                        }
                    }
                }
            }

            return statiSpedizione;
        }

        private bool VerificaAssociazioneClienteSpedizione(string codiceFiscale, string partitaIva, int spedizioneId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Verifica l'associazione utilizzando una query SQL
                string query = "SELECT COUNT(*) FROM Spedizioni \r\nJOIN Clienti ON Spedizioni.ClienteId = Clienti.ClienteId\r\nWHERE (Clienti.CodiceFiscale = @CodiceFiscale OR Clienti.PartitaIva = @PartitaIVA OR Clienti.PartitaIva IS NULL) AND Spedizioni.SpedizioneId = @SpedizioneId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CodiceFiscale", (object)codiceFiscale ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PartitaIva", (object)partitaIva ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SpedizioneId", spedizioneId);

                    int count = (int)command.ExecuteScalar();

                    // Restituisci true se l'associazione è confermata (count > 0), altrimenti false
                    return count > 0;
                }
            }
        }


        public ActionResult AggiornamentiSpedizione(List<AggiornamentoSpedizione> aggiornamenti)
        {
            return View(aggiornamenti);
        }

        public ActionResult NessunaInformazioneSpedizione()
        {
            return View();
        }



    }
}