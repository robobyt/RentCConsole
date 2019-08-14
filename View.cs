using RentCConsole.DbControllers;
using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RentCConsole.Views {
    class View {
        private FormController controller;

        public View(FormController con) {
            controller = con;
        }

        //TODO implement coupons
        // TODO : Add CheckAndClose method. When we try to find available car run this method
        //and check if Now > EndDate and Reservation is not closed or not cancelled then close it and set car available

        /// <summary>
        /// Befor we run the main window and allow customer working there 
        /// we should check if all reservations were closed
        /// </summary>
        public void Process() {
            controller.CheckingUnclosedReservations();

            Console.WriteLine("Welcome to RentC, your brand new solution " +
               "to manage and control your company's data without missing anything");

            Console.WriteLine("Press ENTER to continue or ESC to quit");

            while (true) {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                    MenuScreen();
                else if (keyInfo.Key == ConsoleKey.Escape)
                    Environment.Exit(0);
                else 
                    continue;
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
                        controller.AddCarReservation();
                        GoToMainMenu();
                        break;
                    case "2":
                        controller.UpdateReservation();
                        GoToMainMenu();
                        break;
                    case "3":
                        var table = TablePrinter.GetDataInTableFormat(controller.GoldAndSilverCustomers(2));
                        Console.WriteLine(table);
                        
                        //ListReservations();
                        break;
                    case "4":
                        CarsList();
                        break;
                    case "5":
                        controller.AddCustomer();
                        GoToMainMenu();
                        break;
                    case "6":
                        controller.UpdateCustomer();
                        GoToMainMenu();
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

        private void CarsList() {
            int number = 0;
            bool id = true;
            bool plate = false;
            bool manufacturer = false;
            bool model = false;
            bool price = false;
            bool location = false;

            CarWebService cars = new CarWebService();
            while (true) {
                Console.Clear();
                var table = TablePrinter.GetDataInTableFormat(cars.WebServiceCarsList(number));
                Console.WriteLine(table);

                Console.WriteLine("Sort by: _                                 1 - Car ID");
                Console.WriteLine("                                           2 - Plate");
                Console.WriteLine("                                           3 - Manufacturer");
                Console.WriteLine("                                           4 - Model");
                Console.WriteLine("                                           5 - Price Per Day");
                Console.WriteLine("                                           6 - Location");
                Console.WriteLine("                                           ESC to Main menu");
                number = Utility.InputAndValidatInt();

                switch (number) {
                    case 1:
                        if (id == false) {
                            number = 0;
                            id = true;
                        }
                        else {
                            number = 1;
                            id = false;
                        }
                        break;
                    case 2:
                        if (plate == false) {
                            number = 2;
                            plate = true;
                        }
                        else {
                            number = 3;
                            plate = false;
                        }
                        break;
                    case 3:
                        if (manufacturer == false) {
                            number = 4;
                            manufacturer = true;
                        }
                        else {
                            number = 5;
                            manufacturer = false;
                        }
                        break;
                    case 4:
                        if (model == false) {
                            number = 6;
                            model = true;
                        }
                        else {
                            number = 7;
                            model = false;
                        }
                        break;
                    case 5:
                        if (price == false) {
                            number = 8;
                            price = true;
                        }
                        else {
                            number = 9;
                            price = false;
                        }
                        break;
                    case 6:
                        if (location == false) {
                            number = 10;
                            location = true;
                        }
                        else {
                            number = 11;
                            location = false;
                        }
                        break;
                    default:
                        number = 0;
                        break;

                }

            }
        }


        private void CustomerList() {
            int number = 0;
            bool id = true;
            bool name = false;
            bool birthDate = false;
            bool location = false;

            while (true) {
                Console.Clear();
                var table = TablePrinter.GetDataInTableFormat(controller.CustomerList(number));
                Console.WriteLine(table);

                Console.WriteLine("Sort by: _                                 1 - ClientID");
                Console.WriteLine("                                           2 - Client name");
                Console.WriteLine("                                           3 - Birth date");
                Console.WriteLine("                                           4 - Location");
                Console.WriteLine("                                           ESC to Main menu");
                number = Utility.InputAndValidatInt();

                switch (number) {
                    case 1:
                        if (id == false) {
                            number = 0;
                            id = true;
                        }
                        else {
                            number = 1;
                            id = false;
                        }
                        break;
                    case 2:
                        if (name == false) {
                            number = 2;
                            name = true;
                        }
                        else {
                            number = 3;
                            name = false;
                        }
                        break;
                    case 3:
                        if (birthDate == false) {
                            number = 4;
                            birthDate = true;
                        }
                        else {
                            number = 5;
                            birthDate = false;
                        }
                        break;
                    case 4:
                        if (location == false) {
                            number = 6;
                            location = true;
                        }
                        else {
                            number = 7;
                            location = false;
                        }
                        break;
                    default:
                        number = 0;
                        break;

                }

            }

        }

        private void ListReservations() {
            int number = 0;
            bool plate = true;
            bool clientId = false;
            bool start = false;
            bool end = false;
            bool location = false;

            while (true) {
                Console.Clear();
                var table = TablePrinter.GetDataInTableFormat(controller.ReservationsList(number)); 
                Console.WriteLine(table);

                Console.WriteLine("Sort by: _                                 1 - Car plate");
                Console.WriteLine("                                           2 - Client ID");
                Console.WriteLine("                                           3 - Start Date");
                Console.WriteLine("                                           4 - End Date");
                Console.WriteLine("                                           5 - Location");
                Console.WriteLine("                                           ESC to Main menu");
                number = Utility.InputAndValidatInt();

                switch (number) {
                    case 1:
                        if (plate == false) {
                            number = 0;
                            plate = true;
                        }
                        else {
                            number = 1;
                            plate = false;
                        }
                        break;
                    case 2:
                        if (clientId == false) {
                            number = 2;
                            clientId = true;
                        }
                        else {
                            number = 3;
                            clientId = false;
                        }
                        break;
                    case 3:
                        if (start == false) {
                            number = 4;
                            start = true;
                        }
                        else {
                            number = 5;
                            start = false;
                        }
                        break;
                    case 4:
                        if (end == false) {
                            number = 6;
                            end = true;
                        }
                        else {
                            number = 7;
                            end = false;
                        }
                        break;
                    case 5:
                        if (location == false) {
                            number = 8;
                            location = true;
                        }
                        else {
                            number = 9;
                            location = false;
                        }
                        break;
                    default:
                        number = 0;
                        break;

                }

            }
        }

        private void GoToMainMenu() {
            Console.WriteLine("Press any key");
            Console.ReadKey();
            MenuScreen();
        }

       
        
       
    }
}
