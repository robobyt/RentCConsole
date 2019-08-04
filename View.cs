using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.Views {
    class View {
        private DbController controller;

        public View(DbController con) {
            controller = con;
        }

        public void Process() {
            Console.WriteLine("Welcome to RentC, your brand new solution " +
               "to manage and control your company's data without missing anything");

            Console.WriteLine("Press ENTER to continue or ESC to quit");

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.Enter)
                MenuScreen();
            else if (keyInfo.Key == ConsoleKey.Escape)
                Environment.Exit(0);
        }

        private void MenuScreen() {
            Console.Clear();
            Console.WriteLine("1 Register new Car Rent");
            Console.WriteLine("2 Update Car Rent");
            Console.WriteLine("3 List Rents");
            Console.WriteLine("4 List Available Cars");
            Console.WriteLine("5 Register new Customer");
            Console.WriteLine("6 Update Customer");
            Console.WriteLine("7 List Customers");
            Console.WriteLine("8 Quit");
            while (true) {
                var key = Console.ReadLine().ToString();

                switch (key) {
                    case "1":
                        Console.Clear();
                        controller.AddCarRent(ReservationForm());
                        Console.ReadKey();
                        MenuScreen();
                        break;
                    case "2":
                        break;
                    case "3":
                        Console.Clear();
                        controller.RentsList();
                        break;
                    case "4":
                        break;
                    case "5":
                        Console.Clear();
                        controller.AddCustomer(CustomerForm());
                        Console.ReadKey();
                        MenuScreen();
                        break;
                    case "6":
                        Console.Clear();
                        UpdateCustomer();
                        Console.ReadKey();
                        MenuScreen();
                        break;
                    case "7":
                        Console.Clear();
                        controller.CustomersList();
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Please, choose 1-8 number");
                        break;

                }
            }
        }

        private void UpdateCustomer() {
            Console.WriteLine("Enter Client Id to update");
            int id = Convert.ToInt32(Console.ReadLine());
            if (controller.FindCustomerById(id)) {
                controller.UpdateCustomer(CustomerForm(), id);
            }
            else {
                Console.WriteLine("Customer was not updated");
                Console.ReadKey();
                MenuScreen();
            }
        }

        private Customers CustomerForm() {
            Console.Clear();

            Console.WriteLine("Enter Client Name");
            string name = Console.ReadLine().ToString();

            Console.WriteLine("Enter Client Birthdate");
            DateTime birthDate = Convert.ToDateTime(Console.ReadLine());

            Console.WriteLine("Enter Client ZIP code");
            string location = Console.ReadLine();

            Customers customer = new Customers {
               Name = name,
               BirthDate = birthDate,
               Location = location 
            };
            return customer;
        }

        private Reservations ReservationForm() {
            Console.Clear();

            Console.WriteLine("Enter Client ID");
            int id = Convert.ToInt32(Console.ReadLine().ToString());

            Console.WriteLine("Enter CarID");
            int plate = Convert.ToInt32(Console.ReadLine().ToString());

            Console.WriteLine("Enter Start date");
            DateTime startDate = Convert.ToDateTime(Console.ReadLine());

            Console.WriteLine("Enter End date");
            DateTime endDate = Convert.ToDateTime(Console.ReadLine());

            Console.WriteLine("Enter Client location");
            string location = Console.ReadLine();

            Reservations reservation = new Reservations {
                CustomerID = id,
                CarID = plate,
                StartDate = startDate,
                EndDate = endDate, 
                Location = location
            };

            return reservation;
        }

    }
}
