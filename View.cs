using RentCConsole.Models;
using System;

namespace RentCConsole.Views {
    class View {
        private DbController controller;

        public View(DbController con) {
            controller = con;
        }

        // TODO : Add CheckAndClose method. When we try to find available car run this method
        //and check if Now > EndDate and Reservation is not closed or not cancelled then close it and set car available

        public void Process() {
            Console.WriteLine("Welcome to RentC, your brand new solution " +
               "to manage and control your company's data without missing anything");

            Console.WriteLine("Press ENTER to continue or ESC to quit");

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while (true) {
                if (keyInfo.Key == ConsoleKey.Enter)
                    MenuScreen();
                else if (keyInfo.Key == ConsoleKey.Escape)
                    Environment.Exit(0);
            }

        }

        private void MenuScreen() {
            Console.Clear();
            Console.WriteLine("1 Register new Car Reservation");
            Console.WriteLine("2 Update Car Reservation");
            Console.WriteLine("3 List Reservations");
            Console.WriteLine("4 List Available Cars");
            Console.WriteLine("5 Register new Customer");
            Console.WriteLine("6 Update Customer");
            Console.WriteLine("7 List Customers");
            Console.WriteLine("8 Quit");

            while (true) {
                var key = Console.ReadLine().ToString();

                switch (key) {
                    case "1":
                        AddCarReservation();
                        break;
                    case "2":
                        UpdateReservation();
                        break;
                    case "3":
                        ListReservations();
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

        private void ListReservations() {
            Console.Clear();
            controller.ReservationsList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void AddCarReservation() {
            Console.Clear();

            controller.AddReservation(ReservationForm());

            Console.WriteLine("New Reservation added successful");
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
                Console.WriteLine("Reservation was not updated");
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void UpdateReservation() {
            Console.Clear();

            int customerId = 0;
            int carId = 0;
            string plate = null;
            DateTime startDate = DateTime.Now;

            Console.WriteLine("Enter data for searching reservation:");
            Console.WriteLine("-----------------------");
            Console.WriteLine("Enter Client ID");

            customerId = InputAndValidatInt();
            customerId = CheckIfCustomerExist(customerId);

            Console.WriteLine("Enter Car plate number");
            plate = Console.ReadLine().ToString();
            plate = CheckIfPlateExist(plate);

            carId = controller.FindCarByPlate(plate);

            Console.WriteLine("Enter Start date");
            startDate = InputAndValidatDateTime();

            CheckIfReservationExist(ref customerId, ref carId, ref plate, startDate);

            Console.WriteLine("Press any key to change reservation");
            Console.ReadKey();

            var reservation = ReservationForm();

            Console.WriteLine("If you wnt to cancel reservation hit 1 otherwise any button");
            int cancel = InputAndValidatInt();

            reservation.ReservStatsID = cancel == 1 ? 3 :  1;

            controller.UpdateReservation(reservation, customerId, carId, startDate); 

            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private Customers CustomerForm() {
            Console.Clear();

            DateTime birthDate = DateTime.Now;
            string location = null;

            Console.WriteLine("Enter Client Name");
            string name = Console.ReadLine().ToString();

            Console.WriteLine("Enter Client Birthdate");

            birthDate = CheckIfClientIsAdult(birthDate);

            Console.WriteLine("Enter Client location code");
            location = Console.ReadLine();

            Customers customer = new Customers {
                Name = name,
                BirthDate = birthDate,
                Location = location
            };
            return customer;
        }

        private DateTime CheckIfClientIsAdult(DateTime birthDate) {
            while (DateTime.Now.Year - birthDate.Year < 18) {
                birthDate = InputAndValidatDateTime();
                Console.WriteLine("Client has to be turn 18 yars");
            }

            return birthDate;
        }

        private Reservations ReservationForm() {
            Console.Clear();

            int customerId = 0;
            int carId = 0;
            string plate = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            string location = null;

            Console.WriteLine("Enter Client ID");

            customerId = InputAndValidatInt();
            customerId = CheckIfCustomerExist(customerId);

            Console.WriteLine("Enter Car plate number");
            plate = Console.ReadLine().ToString();
            plate = CheckIfPlateExist(plate);

            carId = controller.FindCarByPlate(plate);

            Console.WriteLine("Enter Start date");
            startDate = InputAndValidatDateTime();
            startDate = CheckIfCorrectDate(startDate);

            Console.WriteLine("Enter End date");
            endDate = InputAndValidatDateTime();
            startDate = CheckIfEndDateIsCorrect(startDate, endDate);

            Console.WriteLine("Enter Client location");
            location = Console.ReadLine().ToString();

            Reservations reservation = new Reservations {
                CustomerID = customerId,
                CarID = carId,
                StartDate = startDate,
                EndDate = endDate,
                Location = location,
                ReservStatsID = 1
            };

            return reservation;
        }

        private void CheckIfReservationExist(ref int customerId, ref int carId, ref string plate, DateTime startDate) {
            bool checkReservExist = controller.FindReservationByKeys(customerId, carId, startDate);

            while (!checkReservExist) {
                Console.WriteLine("Reservation with customer ID {0} " +
                    "and start date {1} doesn't exist exist! Please, enter another data" +
                    " presss ESC to back Main menu", customerId, startDate);

                Console.WriteLine("Enter Client ID");

                customerId = InputAndValidatInt();
                customerId = CheckIfCustomerExist(customerId);

                Console.WriteLine("Enter Car plate number");
                plate = Console.ReadLine().ToString();
                plate = CheckIfPlateExist(plate);

                carId = controller.FindCarByPlate(plate);

                Console.WriteLine("Enter Start date");
                startDate = InputAndValidatDateTime();

                checkReservExist = controller.FindReservationByKeys(customerId, carId, startDate);
            }
        }

        private DateTime CheckIfEndDateIsCorrect(DateTime startDate, DateTime endDate) {
            while (endDate < startDate) {
                Console.WriteLine("End date can't be earlier then start date");
                startDate = InputAndValidatDateTime();
                endDate = InputAndValidatDateTime();
            }

            return startDate;
        }

        private DateTime CheckIfCorrectDate(DateTime startDate) {
            while (startDate < DateTime.Now) {
                Console.WriteLine("Please, set date not earlier then today");
                startDate = InputAndValidatDateTime();
            }

            return startDate;
        }

        private int CheckIfCustomerExist(int customerId) {
            bool exist = controller.FindCustomerById(customerId);
            while (!exist) {
                Console.WriteLine("Customer with id: {0} doesn't exist", customerId);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                customerId = InputAndValidatInt();
                exist = controller.FindCustomerById(customerId);
            };
            return customerId;
        }

        private string CheckIfPlateExist(string plate) {
            int car = controller.FindCarByPlate(plate);
            while (car < 1) {
                Console.WriteLine("Car with number: {0} doesn't exist", plate);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                plate = Console.ReadLine().ToString();
                car = controller.FindCarByPlate(plate);
            }

            return plate;
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
