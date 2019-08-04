using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RentCConsole.Models;
using RentCConsole.Views;

namespace RentCConsole {
    class Program {
        

        static void Main(string[] args) {
            Settings();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            DbController controller = new DbController(connection);
            View view = new View(controller);
            view.Process();

        }

        public static void Settings() {
            Console.Title = "RentC";
            //Console.SetWindowSize(100, 100);
            //Console.SetBufferSize(100, 100);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
        }

       

    }
}
