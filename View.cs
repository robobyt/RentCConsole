using RentCConsole.DbControllers;
using RentCConsole.Models;
using System;
using System.Data.SqlClient;

namespace RentCConsole.Views {
    class View {
        private SqlConnection connection;

        private CarController carController;
        private ReservationController reservationController;
        private CustomerController customerController;

        public View(SqlConnection con) {
            connection = con;
            carController = new CarController(connection);
            customerController = new CustomerController(connection);
            reservationController = new ReservationController(connection);
        }

        // TODO : Add CheckAndClose method. When we try to find available car run this method
        //and check if Now > EndDate and Reservation is not closed or not cancelled then close it and set car available

        /// <summary>
        /// Befor we run the main window and allow customer working there 
        /// we should check if all reservations were closed
        /// </summary>
        public void Process() {
            CheckingUnclosedReservations();

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

        /// <summary>
        /// Every day checking if in the database are unclosed reservations
        /// </summary>
        private void CheckingUnclosedReservations() {
            //reservationController.CheckingUnclosedReservations();
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
            TablePrinter.GetDataInTableFormat(carController.CarsList());
            GoToMainMenu();
        }

        private void CustomerList() {
            Console.Clear();
            TablePrinter.GetDataInTableFormat(customerController.CustomerList());
            GoToMainMenu();
        }

        private void ListReservations() {
            Console.Clear();
            TablePrinter.GetDataInTableFormat(reservationController.ReservationsList());
            GoToMainMenu();
        }

        private void GoToMainMenu() {
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void AddCarReservation() {
            Console.Clear();

            reservationController.AddReservation(ReservationForm());

            Console.WriteLine("New Reservation added successful");
            GoToMainMenu();
        }

        private void AddCustomer() {
            Console.Clear();
            customerController.AddCustomer(CustomerForm());
            GoToMainMenu();
        }

        private void UpdateCustomer() {
            Console.Clear();

            Console.WriteLine("Enter Client Id to update");
            int id = Utility.InputAndValidatInt();

            CheckIfCustomerExist(id);

            if (customerController.FindCustomerById(id)) {
                customerController.UpdateCustomer(CustomerForm(), id);
            }
            else {
                Console.WriteLine("Reservation was not updated");
            }

            GoToMainMenu();
        }

        /// <summary>
        /// Run first form to input ID's for checking is reservation exists.
        /// Next ask customer if he wants canel the reservation. If true
        /// run CancelReservation. If not run the reservation form and next run UpdateReservation
        /// </summary>
        private void UpdateReservation() {
            Console.Clear();

            int customerId = 0;
            int carId = 0;
            string location = null;
            Reservations reservation;

            DateTime startDate = DateTime.Now;

            Console.WriteLine("Enter data for searching reservation:");
            Console.WriteLine("-----------------------");

            CustomerIdToLocation(ref customerId, ref location);
            carId = CarPlateToCarId();

            Console.WriteLine("Please input start date of reservation");
            startDate = Utility.InputAndValidatDateTime();

            CheckIfReservationExist(ref customerId, ref carId, startDate);

            Console.WriteLine("Press any key to change reservation");
            Console.ReadKey();

            Console.WriteLine("If you wnt to cancel reservation hit 1 otherwise any button");
            int cancel = Utility.InputAndValidatInt();

            if(cancel == 1) {
                reservationController.CancelReservation(customerId, carId, startDate);
                GoToMainMenu();
            }

            reservation = ReservationForm();
            reservationController.UpdateReservation(reservation, customerId, carId, startDate);

            GoToMainMenu();
        }

        /// <summary>
        /// Create Customers instance from inputs data
        /// </summary>
        /// <returns></returns>
        private Customers CustomerForm() {
            Console.Clear();

            DateTime birthDate = DateTime.Now;
            string location = null;

            Console.WriteLine("Enter Client Name");
            string name = Console.ReadLine().ToString();

            Console.WriteLine("Enter Client Birthdate");

            birthDate = Utility.CheckIfClientIsAdult(birthDate);

            Console.WriteLine("Enter Client location code");
            location = Console.ReadLine();

            Customers customer = new Customers {
                Name = name,
                BirthDate = birthDate,
                Location = location
            };
            return customer;
        }

        /// <summary>
        /// Create Reservations instance from inputs data.
        /// </summary>
        /// <returns></returns>
        private Reservations ReservationForm() {
            Console.Clear();
            int customerId = 0;
            string location = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            CustomerIdToLocation(ref customerId, ref location);

            int carId = CarPlateToCarId();

            CheckIfCarAvailableAtLocation(ref carId, ref location);

            startDate = Utility.CheckIfCorrectDate(startDate);

            Console.WriteLine("Enter End date");
            endDate = Utility.InputAndValidatDateTime();
            startDate = Utility.CheckIfEndDateIsCorrect(startDate, endDate);

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

        /// <summary>
        /// Ask user to input car plate. If car with this plate exists method retirns car ID
        /// </summary>
        /// <returns></returns>
        private int CarPlateToCarId() {
            Console.WriteLine("Enter Car plate number");
            string plate = Console.ReadLine().ToString();
            int carId = CheckIfPlateExist(plate);
            return carId;
        }

        /// <summary>
        /// Ask user to input customer ID. If customer with this ID exists method 
        /// returns customer location
        /// </summary>
        /// <returns></returns>
        private void CustomerIdToLocation(ref int customerId, ref string location) {
            Console.WriteLine("Enter Client ID");
            customerId = Utility.InputAndValidatInt();
            location = CheckIfCustomerExist(customerId);
        }

        /// <summary>
        /// Before updating reservation we have to be sure that we have exactly record in the database.
        /// Reservation from one customer for one car in the same city isn't allowed with the same date.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="carId"></param>
        /// <param name="startDate"></param>
        private void CheckIfReservationExist(ref int customerId, ref int carId, DateTime startDate) {
            bool checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);
            string location = null;

            while (!checkReservExist) {
                Console.WriteLine("Reservation with customer ID {0} " +
                    "and start date {1} doesn't exist exist! Please, enter another data" +
                    " presss ESC to back Main menu", customerId, startDate);

                CustomerIdToLocation(ref customerId, ref location);
                carId = CarPlateToCarId();
                startDate = Utility.CheckIfCorrectDate(startDate);

                checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);
            }
        }

        /// <summary>
        /// Checking if the car is available in the same city where customer is
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="location"></param>
        private void CheckIfCarAvailableAtLocation(ref int carId, ref string location) {
            bool checkReservExist = carController.FindAvailableCar(carId, location);
            while (!checkReservExist) {
                Console.WriteLine("There are no cars in {0}! Please, enter another plate number " +
                    "or presss ESC to back Main menu", location);
                carId = CarPlateToCarId();
                checkReservExist = carController.FindAvailableCar(carId, location);
            }
        }

        /// <summary>
        /// Chiking customer ID inputs from customers. If it is correct returns customer location
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        private string CheckIfCustomerExist(int customerId) {
            string location = customerController.FindCustomerLocationById(customerId);
            while (location == null) {
                Console.WriteLine("Customer with id: {0} doesn't exist", customerId);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                customerId = Utility.InputAndValidatInt();
                location = customerController.FindCustomerLocationById(customerId);
            };
            return location;
        }

        /// <summary>
        /// Chiking car plate inputs from customers. If it is correct returns car ID
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        private int CheckIfPlateExist(string plate) {
            int car = carController.FindCarByPlate(plate);
            while (car < 1) {
                Console.WriteLine("Car with number: {0} doesn't exist", plate);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                plate = Console.ReadLine().ToString();
                car = carController.FindCarByPlate(plate);
            }

            return car;
        }
       
    }
}
