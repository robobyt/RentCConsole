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
                        AddCarRent();
                        break;
                    case "2":
                        break;
                    case "3":
                        ListRents();
                        break;
                    case "4":
                        ListCars();
                        break;
                    case "5":
                        AddCustomer();
                        break;
                    case "6":
                        UpdateCustomer();
                        break;
                    case "7":
                        CustomerList();
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

        private void ListCars() {
            Console.Clear();
            controller.CarsList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void ListRents() {
            Console.Clear();
            controller.RentsList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void AddCarRent() {
            Console.Clear();

            controller.AddCarRent(ReservationForm());

            Console.WriteLine("New Rent added successful");
            Console.WriteLine("Press any key");
            Console.ReadKey();

            MenuScreen();
        }

        private void AddCustomer() {
            Console.Clear();
            controller.AddCustomer(CustomerForm());
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void CustomerList() {
            Console.Clear();
            controller.CustomersList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void UpdateCustomer() {
            Console.Clear();

            Console.WriteLine("Enter Client Id to update");
            int id = InputAndValidatInt();

            if (controller.FindCustomerById(id)) {
                controller.UpdateCustomer(CustomerForm(), id);
            }
            else {
                Console.WriteLine("Customer was not updated");
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        // TO DO
        private void UpdateRent() {
            Console.Clear();

            Console.WriteLine("Enter Rent Id to update");
            int id = InputAndValidatInt();

            if (controller.FindCustomerById(id)) {
                controller.UpdateCustomer(CustomerForm(), id);
            }
            else {
                Console.WriteLine("Customer was not updated");
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private Customers CustomerForm() {
            Console.Clear();

            string name = null;
            DateTime birthDate = DateTime.Now;
            string location = null;

            while (true) {
                Console.WriteLine("Enter Client Name");
                name = Console.ReadLine().ToString();

                Console.WriteLine("Enter Client Birthdate");
                birthDate = InputAndValidatDateTime();

                if (birthDate.Year - DateTime.Now.Year <= 18) {
                    Console.WriteLine("Customer has turn 18 yars");
                    continue;
                }

                Console.WriteLine("Enter Client location code");
                location = Console.ReadLine();

                break;
            }

            Customers customer = new Customers {
               Name = name,
               BirthDate = birthDate,
               Location = location 
            };
            return customer;
        }

        private Reservations ReservationForm() {
            Console.Clear();

            int customerId = 0;
            int carId = 0;
            string plate = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            string location = null;

            while (true) {
                Console.WriteLine("Enter Client ID");
                customerId = InputAndValidatInt();

                if (!controller.FindCustomerById(customerId)){
                    Console.WriteLine("Customer with id: {0} doesn't exist", customerId);
                    continue;
                };

                Console.WriteLine("Enter Car plate number");
                plate = Console.ReadLine().ToString();

                if (controller.FindCarByPlate(plate) < 1) {
                    Console.WriteLine("Car with number: {0} doesn't exist", plate);
                    continue;
                };

                carId = controller.FindCarByPlate(plate);

                Console.WriteLine("Enter Start date");
                startDate = InputAndValidatDateTime();

                if(startDate.Ticks < DateTime.Now.Ticks) {
                    Console.WriteLine("Please, set date not earler then today");
                    continue;
                }

                Console.WriteLine("Enter End date");
                endDate = InputAndValidatDateTime();

                if (endDate.CompareTo(startDate) < 0) {
                    Console.WriteLine("End date hasn't be earlier then start date");
                    continue;
                }

                Console.WriteLine("Enter Client location");
                location = Console.ReadLine().ToString();
                break;
            }



            Reservations reservation = new Reservations {
                CustomerID = customerId,
                CarID = carId,
                CarPlate = plate,
                StartDate = startDate,
                EndDate = endDate, 
                Location = location
            };

            return reservation;
        }

        private DateTime InputAndValidatDateTime() {
            DateTime date = DateTime.UtcNow;
            while (!DateTime.TryParse(Console.ReadLine(), out date)) {
                Console.WriteLine("Please enter date in format: dd.mm.year");
            }
            return date;
        }

        private int InputAndValidatInt() {
            int number = 0;
            while(!int.TryParse(Console.ReadLine(), out number)) {
                Console.WriteLine("Please enter positive number");
            }
            return number;
        }

        private void WarningMessage(string str) {

        }

        private void ErrorMessage(string str) {

        }
    }
}
