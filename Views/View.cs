﻿using RentCConsole.DbControllers;
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
            carController.CarsList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void ListReservations() {
            Console.Clear();
            reservationController.ReservationsList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void AddCarReservation() {
            Console.Clear();

            reservationController.AddReservation(ReservationForm());

            Console.WriteLine("New Reservation added successful");
            Console.WriteLine("Press any key");
            Console.ReadKey();

            MenuScreen();
        }

        private void AddCustomer() {
            Console.Clear();
            customerController.AddCustomer(CustomerForm());
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void CustomerList() {
            Console.Clear();
            customerController.CustomerList();
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void UpdateCustomer() {
            Console.Clear();

            Console.WriteLine("Enter Client Id to update");
            int id = Utility.InputAndValidatInt();

            if (customerController.FindCustomerById(id)) {
                customerController.UpdateCustomer(CustomerForm(), id);
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

            GatherSignatureForReservation(out customerId, out carId, out plate, out startDate);

            CheckIfReservationExist(ref customerId, ref carId, ref plate, startDate);

            Console.WriteLine("Press any key to change reservation");
            Console.ReadKey();

            var reservation = ReservationForm();

            Console.WriteLine("If you wnt to cancel reservation hit 1 otherwise any button");
            int cancel = Utility.InputAndValidatInt();

            reservation.ReservStatsID = cancel == 1 ? 3 : 1;

            reservationController.UpdateReservation(reservation, customerId, carId, startDate);

            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

        private void GatherSignatureForReservation(out int customerId, out int carId, out string plate, out DateTime startDate) {
            Console.WriteLine("Enter Client ID");

            customerId = Utility.InputAndValidatInt();
            customerId = CheckIfCustomerExist(customerId);

            Console.WriteLine("Enter Car plate number");
            plate = Console.ReadLine().ToString();
            plate = CheckIfPlateExist(plate);

            carId = carController.FindCarByPlate(plate);

            Console.WriteLine("Enter Start date");
            startDate = Utility.InputAndValidatDateTime();
        }

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

        

        private Reservations ReservationForm() {
            Console.Clear();

            int customerId = 0;
            int carId = 0;
            string plate = null;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            string location = null;

            GatherSignatureForReservation(out customerId, out carId, out plate, out startDate);

            startDate = Utility.CheckIfCorrectDate(startDate);

            Console.WriteLine("Enter End date");
            endDate = Utility.InputAndValidatDateTime();
            startDate = Utility.CheckIfEndDateIsCorrect(startDate, endDate);

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
            bool checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);

            while (!checkReservExist) {
                Console.WriteLine("Reservation with customer ID {0} " +
                    "and start date {1} doesn't exist exist! Please, enter another data" +
                    " presss ESC to back Main menu", customerId, startDate);

                GatherSignatureForReservation(out customerId, out carId, out plate, out startDate);

                checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);
            }
        }


        private int CheckIfCustomerExist(int customerId) {
            bool exist = customerController.FindCustomerById(customerId);
            while (!exist) {
                Console.WriteLine("Customer with id: {0} doesn't exist", customerId);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                customerId = Utility.InputAndValidatInt();
                exist = customerController.FindCustomerById(customerId);
            };
            return customerId;
        }

        private string CheckIfPlateExist(string plate) {
            int car = carController.FindCarByPlate(plate);
            while (car < 1) {
                Console.WriteLine("Car with number: {0} doesn't exist", plate);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                plate = Console.ReadLine().ToString();
                car = carController.FindCarByPlate(plate);
            }

            return plate;
        }
       
    }
}