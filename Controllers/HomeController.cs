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

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<string> users = new List<string>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT `Name` FROM `user`";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString("Name");
                                users.Add(name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return NotFound();
            }

            ViewBag.Users = users;
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

        [HttpPost]
        public IActionResult DeleteMySQL(string userName)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM `user` WHERE `Name` = @Name";

                    using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", userName);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ViewBag.Text = $"刪除使用者 {userName} 成功！";
                        }
                        else
                        {
                            ViewBag.Text = $"找不到使用者 {userName}。";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = $"出現錯誤：{ex.Message}";
                return NotFound();
            }

            // 重新導向到 Index 頁面，以顯示最新的使用者清單
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}