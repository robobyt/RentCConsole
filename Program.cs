﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RentCConsole.Models;
using RentCConsole.Views;
using System.Threading;

namespace RentCConsole {
    class Program {
        

        static void Main(string[] args) {
            Settings();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            FormController controller = new FormController(connection);
            View view = new View(controller);

            view.Process();

        }


        public static void Settings() {
            Console.Title = "RentC";
            //Console.SetWindowSize(100, 100);
            //Console.SetBufferSize(100, 100);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
        }

       

    }
}
