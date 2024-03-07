using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class RaggruppamentoSpedizionitController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Fedex"].ToString();
        // GET: RaggruppamentoSpedizionit
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> SpedizioniInConsegnaOdierna()
        {
            DateTime dataOdierna = DateTime.Now;
            List<Spedizione> spedizioniInConsegna = await GetSpedizioniInConsegnaAsync(dataOdierna);
            return View("SpedizioniInConsegnaOdierna", spedizioniInConsegna);
        }

        public async Task<List<Spedizione>> GetSpedizioniInConsegnaAsync(DateTime dataConsegna)
        {
            List<Spedizione> spedizioniInConsegna = new List<Spedizione>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Crea una query SQL per ottenere le spedizioni della data odierna
                string query = "SELECT * FROM Spedizioni WHERE CONVERT(DATE, DataConsegnaPrevista) = @DataConsegna";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DataConsegna", dataConsegna.Date);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            Spedizione spedizione = new Spedizione
                            {
                                SpedizioneId = (int)reader["SpedizioneId"],
                                ClienteId = (int)reader["ClienteId"],
                                // ... altre proprietà della spedizione
                            };

                            spedizioniInConsegna.Add(spedizione);
                        }
                    }
                }
            }

            return spedizioniInConsegna;
        }






    }
}