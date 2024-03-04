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
                            Spedizione spedizione = new Spedizione
                            {
                                SpedizioneId = (int)reader["SpedizioneId"],
                                ClienteId = (int)reader["ClienteId"],
                                DataSpedizione = (DateTime)reader["DataSpedizione"],
                                Peso = (int)reader["Peso"],
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                                CostoSpedizione = (decimal)reader["CostoSpedizione"],
                                DataConsegnaPrevista = (DateTime)reader["DataSpedizionePrevista"]

                            };

                            listaSpedizioni.Add(spedizione);
                        }
                    }
                }
            }

            return listaSpedizioni;
        }
    }
}