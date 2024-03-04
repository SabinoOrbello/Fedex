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
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Fedex"].ToString();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaClienti()
        {
            List<Clienti> listaClienti = GetListaClientiFromDatabase();

            return View(listaClienti);
        }

        private List<Clienti> GetListaClientiFromDatabase()
        {
            List<Clienti> listaClienti = new List<Clienti>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Clienti", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Clienti cliente = new Clienti
                            {
                                ClienteId = (int)reader["ClienteId"],
                                Nome = (string)reader["Nome"],
                                CodiceFiscale = reader["CodiceFiscale"].ToString(),
                                PartitaIva = reader["PartitaIva"].ToString(),
                                TipoCliente = reader["TipoCliente"].ToString()
                            };

                            listaClienti.Add(cliente);
                        }
                    }
                }
            }

            return listaClienti;
        }

        public ActionResult AnagrafaCliente()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AnagrafaCliente(Clienti cliente)
        {
            if (ModelState.IsValid)
            {

                InserisciClienteNelDatabase(cliente);
                return RedirectToAction("AnagrafaCliente");
            }

            return View(cliente);
        }

        private void InserisciClienteNelDatabase(Clienti cliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Clienti (Nome, CodiceFiscale, PartitaIva, TipoCliente) VALUES (@Nome, @CodiceFiscale, @PartitaIva, @TipoCliente)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nome", cliente.Nome);
                    command.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale);

                    // Aggiungi @PartitaIva solo se è stato fornito un valore
                    if (!string.IsNullOrWhiteSpace(cliente.PartitaIva))
                    {
                        command.Parameters.AddWithValue("@PartitaIva", cliente.PartitaIva);
                    }
                    else
                    {
                        // Altrimenti, imposta il parametro a DBNull.Value o a un valore vuoto a seconda del tuo database
                        command.Parameters.AddWithValue("@PartitaIva", DBNull.Value);
                        // oppure command.Parameters.AddWithValue("@PartitaIva", string.Empty);
                    }

                    command.Parameters.AddWithValue("@TipoCliente", cliente.TipoCliente);

                    command.ExecuteNonQuery();
                }
            }
        }




    }
}