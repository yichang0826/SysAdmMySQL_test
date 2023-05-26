using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using SysAdmMySQL.Models;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

using System.Xml.Linq;

namespace SysAdmMySQL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertMySQL(string userName)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string insertQuery = "INSERT INTO `user` (`Name`) VALUES (@Name)";

                MySqlCommand command = new MySqlCommand(insertQuery, connection);

                // 設定要插入的資料值
                command.Parameters.AddWithValue("@Name", userName);
                int rowsAffected = command.ExecuteNonQuery();

                ViewBag.Text = $"插入 {rowsAffected} 筆資料成功！";
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return NotFound();
            }
            return RedirectToAction("Index");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}