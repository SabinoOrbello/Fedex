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

        string connectionString = ConfigurationManager.ConnectionStrings["Fedex"].ToString();

        public ActionResult ListaStatoSpedizioni()
        {
            List<AggiornamentoSpedizione> listaStatoSpedizioni = GetListaStatoSpedizioniFromDatabase();

            return View(listaStatoSpedizioni);
        }

        public ActionResult Create()
        {
            return View();
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

        [HttpPost]
        public ActionResult Create(AggiornamentoSpedizione nuovoStatoSpedizione)
        {
            if (ModelState.IsValid)
            {

                InserisciSpedizioneNelDatabase(nuovoStatoSpedizione);

                return RedirectToAction("ListaStatoSpedizioni");
            }


            return View(nuovoStatoSpedizione);
        }

        private void InserisciSpedizioneNelDatabase(AggiornamentoSpedizione nuovoStatoSpedizione)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO AggiornamentiSpedizioni (SpedizioneId, Stato, Luogo, Descrizione, DataOraAggiornamento) VALUES (@SpedizioneId, @Stato, @Luogo, @Descrizione, @DataOraAggiornamento)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SpedizioneId", nuovoStatoSpedizione.SpedizioneId);
                    command.Parameters.AddWithValue("@Stato", nuovoStatoSpedizione.Stato);
                    command.Parameters.AddWithValue("@Luogo", nuovoStatoSpedizione.Luogo);
                    command.Parameters.AddWithValue("@Descrizione", nuovoStatoSpedizione.Descrizione);
                    command.Parameters.AddWithValue("@DataOraAggiornamento", nuovoStatoSpedizione.DataOraAggiornamento);

                    command.ExecuteNonQuery();
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

        private bool VerificaAssociazioneClienteSpedizione(string codiceFiscale, string partitaIVA, int spedizioneId)
        {
            int clienteId = GetClienteIdByCodiceFiscale(codiceFiscale);

            if (clienteId > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Spedizioni WHERE ClienteId = @ClienteId AND SpedizioneId = @SpedizioneId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", clienteId);
                        command.Parameters.AddWithValue("@SpedizioneId", spedizioneId);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }

            return false;
        }

        private bool VerificaAssociazioneClienteSpedizione1(string codiceFiscale, string partitaIVA, int spedizioneId)
        {
            int clienteId = GetClienteIdByPartitaIVA(partitaIVA);

            if (clienteId > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM Spedizioni WHERE ClienteId = @ClienteId AND SpedizioneId = @SpedizioneId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClienteId", clienteId);
                        command.Parameters.AddWithValue("@SpedizioneId", spedizioneId);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }

            return false;
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
                    if (VerificaAssociazioneClienteSpedizione1(null, model.PartitaIva, model.SpedizioneId))
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


        private int GetClienteIdByCodiceFiscale(string codiceFiscale)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ClienteId FROM Clienti WHERE CodiceFiscale = @CodiceFiscale";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assicurati che il parametro venga aggiunto correttamente
                    command.Parameters.Add("@CodiceFiscale", System.Data.SqlDbType.NVarChar, 16).Value = codiceFiscale;

                    // Esegui la query e restituisci il risultato
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                    return 0;
                }
            }
        }

        private int GetClienteIdByPartitaIVA(string partitaIVA)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ClienteId FROM Clienti WHERE PartitaIVA = @PartitaIVA";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assicurati che il parametro venga aggiunto correttamente
                    command.Parameters.Add("@PartitaIVA", System.Data.SqlDbType.NVarChar, 20).Value = partitaIVA;

                    // Eseguire ExecuteScalar per ottenere l'ID del cliente
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                }
            }

            return 0; // o un valore che rappresenta l'assenza di un cliente con la partita IVA specificata
        }





















    }




}
