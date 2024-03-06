using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedizioni.Models;

namespace Spedizioni.Controllers
{
    public class SpedizioniController : Controller
    {
        // GET: Spedizioni
        string connectionString = ConfigurationManager.ConnectionStrings["Fedex"].ToString();

        public ActionResult ListaSpedizioni()
        {
            List<Spedizione> listaSpedizioni = GetListaSpedizioniFromDatabase();

            return View(listaSpedizioni);
        }

        private List<Spedizione> GetListaSpedizioniFromDatabase()
        {
            List<Spedizione> listaSpedizioni = new List<Spedizione>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Spedizioni", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Spedizione spedizioni = new Spedizione
                            {
                                SpedizioneId = (int)reader["SpedizioneId"],
                                ClienteId = (int)reader["ClienteId"],
                                DataSpedizione = (DateTime)reader["DataSpedizione"],
                                Peso = Convert.ToSingle(reader["Peso"]),
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                                CostoSpedizione = (decimal)reader["CostoSpedizione"],
                                DataConsegnaPrevista = (DateTime)reader["DataConsegnaPrevista"]

                            };

                            listaSpedizioni.Add(spedizioni);
                        }
                    }
                }
            }

            return listaSpedizioni;
        }

        public ActionResult Create()
        {
            return View();
        }

        // Azione per gestire la creazione effettiva di una spedizione
        [HttpPost]
        public ActionResult Create(Spedizione nuovaSpedizione)
        {
            if (ModelState.IsValid)
            {
                // Effettua l'inserimento nel database o altra logica necessaria
                InserisciSpedizioneNelDatabase(nuovaSpedizione);

                // Redirect alla lista delle spedizioni dopo la creazione
                return RedirectToAction("ListaSpedizioni");
            }

            // Se il modello non è valido, torna alla vista di creazione con i dati inseriti
            return View(nuovaSpedizione);
        }

        private void InserisciSpedizioneNelDatabase(Spedizione spedizione)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Spedizioni (ClienteId, DataSpedizione, Peso, CittaDestinataria, IndirizzoDestinatario, NominativoDestinatario, CostoSpedizione, DataConsegnaPrevista) VALUES (@ClienteId, @DataSpedizione, @Peso, @CittaDestinataria, @IndirizzoDestinatario, @NominativoDestinatario, @CostoSpedizione, @DataConsegnaPrevista)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClienteId", spedizione.ClienteId);
                    command.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    command.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    command.Parameters.AddWithValue("@CittaDestinataria", spedizione.CittaDestinataria);
                    command.Parameters.AddWithValue("@IndirizzoDestinatario", spedizione.IndirizzoDestinatario);
                    command.Parameters.AddWithValue("@NominativoDestinatario", spedizione.NominativoDestinatario);
                    command.Parameters.AddWithValue("@CostoSpedizione", spedizione.CostoSpedizione);
                    command.Parameters.AddWithValue("@DataConsegnaPrevista", spedizione.DataConsegnaPrevista);

                    command.ExecuteNonQuery();
                }
            }
        }



    }
}