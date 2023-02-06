using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using uph.dotnet.pz.Models;

#pragma warning disable CS8601 // Possible null reference assignment.
namespace uph.dotnet.pz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        List<Customer> customerList = new List<Customer>();

        SqlConnectionStringBuilder builder = new()
        {
            DataSource = "localhost",
            UserID = "uph",
            Password = "uph123",
            InitialCatalog = "uph",
            Encrypt = false
        };

        SqlConnection? con;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            //using (con = new SqlConnection(builder.ConnectionString))
            using (con = new SqlConnection(_configuration.GetConnectionString("uph")))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM tab_customer", con);
                cmd.CommandType = CommandType.Text;
                await con.OpenAsync();

                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                while (await rdr.ReadAsync())
                {
                    var customer = new Customer();

                    customer.Customer_Id = Convert.ToInt32(rdr["customer_id"]);
                    customer.Firstname = rdr["firstname"].ToString();
                    customer.Lastname = rdr["lastname"].ToString();
                    customer.Email = rdr["email"].ToString();
                    customerList.Add(customer);
                }
                await con.CloseAsync();
            }
            return View(customerList);
        }

        // GET: Home/Delete/x
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            using (con = new SqlConnection(builder.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"DELETE FROM tab_customer WHERE customer_id = {id}", con);
                cmd.CommandType = CommandType.Text;

                await con.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? firstname, string? lastname, string? email)
        {
            string potential_password = "test";
            var sha1 = SHA1.Create();
            string? hash = Convert.ToHexString(sha1.ComputeHash(Encoding.ASCII.GetBytes(potential_password)));

            using (con = new SqlConnection(builder.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand($"INSERT INTO tab_customer (firstname, lastname, email) " +
                                                $"VALUES ('{firstname}', '{lastname}', '{email}');", con);
                cmd.CommandType = CommandType.Text;

                await con.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                await con.CloseAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}