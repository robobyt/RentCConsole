using RentCConsole.DbControllers;
using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole {
    public class FormController {
        private SqlConnection connection;

        private ICarController carController;
        private IReservationController reservationController;
        private ICustomerController customerController;

        public FormController(SqlConnection con) {
            connection = con;
            carController = new CarController(connection);
            customerController = new CustomerController(connection);
            reservationController = new ReservationController(connection);
        }

        public List<string[]> ReservationsList(int number) {
            return reservationController.ReservationsList(number);
        }

        public List<string[]> CustomerList(int number) {
            return customerController.CustomerList(number);
        }

        /// <summary>
        /// Ask user to input car plate. If car with this plate exists method retirns car ID
        /// </summary>
        /// <returns></returns>
        public int CarPlateToCarId() {
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
        public void CustomerIdToLocation(ref int customerId, ref string location) {
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
        public void CheckIfReservationExist(ref int customerId, ref int carId, DateTime startDate) {
            bool checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);
            string location = null;

            while (!checkReservExist) {
                Console.WriteLine("Reservation with customer ID {0} " +
                    "and start date {1} doesn't exist exist! Please, enter another data" +
                    " presss ESC to back Main menu", customerId, startDate);

                CustomerIdToLocation(ref customerId, ref location);
                carId = CarPlateToCarId();

                do {
                    Console.WriteLine("Enter Start date");
                    startDate = Utility.InputAndValidatDateTime();
                } while (!Utility.CheckIfCorrectDate(startDate));

                checkReservExist = reservationController.FindReservationByKeys(customerId, carId, startDate);
            }
        }

        /// <summary>
        /// Checking if the car is available in the same city where customer is
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="location"></param>
        public void CheckIfCarAvailableAtLocation(ref int carId, ref string location) {
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
        public string CheckIfCustomerExist(int customerId) {
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
        public int CheckIfPlateExist(string plate) {
            int car = carController.FindCarByPlate(plate);
            while (car < 1) {
                Console.WriteLine("Car with number: {0} doesn't exist", plate);
                Console.WriteLine("Try one more! To exit in main menu hit ESC");
                plate = Console.ReadLine().ToString();
                car = carController.FindCarByPlate(plate);
            }

            return car;
        }

        public void AddCarReservation() {
            Console.Clear();
            reservationController.AddReservation(Forms.ReservationForm(this));

            Console.WriteLine("New Reservation added successful");
        }

        public void AddCustomer() {
            Console.Clear();
            customerController.AddCustomer(Forms.CustomerForm());
        }

        public void UpdateCustomer() {
            Console.Clear();

            Console.WriteLine("Enter Client Id to update");
            int id = Utility.InputAndValidatInt();

            CheckIfCustomerExist(id);

            if (customerController.FindCustomerById(id)) {
                customerController.UpdateCustomer(Forms.CustomerForm(), id);
            }
            else {
                Console.WriteLine("Reservation was not updated");
            }

        }

        /// <summary>
        /// Run first form to input ID's for checking is reservation exists.
        /// Next ask customer if he wants canel the reservation. If true
        /// run CancelReservation. If not run the reservation form and next run UpdateReservation
        /// </summary>
        public void UpdateReservation() {
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

            if (cancel == 1) {
                reservationController.CancelReservation(customerId, carId, startDate);
            }

            reservation = Forms.ReservationForm(this);
            reservationController.UpdateReservation(reservation, customerId, carId, startDate);

        }

        /// <summary>
        /// Every day checking if in the database are unclosed reservations
        /// </summary>
        public void CheckingUnclosedReservations() {
            reservationController.CloseReservation();
        }

    }
}
