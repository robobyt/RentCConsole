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

        public List<string[]> GoldAndSilverCustomers(int count) {
            return customerController.GoldAndSilverCustomers(count);
        }

        public List<string[]> MostRentedCars(int OrderBy) {
            return carController.MostRentedCarsByMonth(OrderBy);
        }

        public List<string[]> MostRecentCars() {
            return carController.TenMostRecentCars();
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
            while(!CheckIfPlateExist(ref plate)) {
                Console.WriteLine("Enter Car plate number");
                plate = Console.ReadLine().ToString();
            }

            return carController.FindCarByPlate(plate);
        }

        /// <summary>
        /// Ask user to input customer ID. If customer with this ID exists method 
        /// returns customer location
        /// </summary>
        /// <returns></returns>
        public void CustomerIdToLocation(ref int customerId, ref string location) {
            customerId = CheckIfCustomerExist();
            while (customerId < 1) {
                customerId = CheckIfCustomerExist();
            }
            location = customerController.FindCustomerLocationById(customerId);
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
                Utility.ErrorMessage($"Reservation with customer ID {customerId} " +
                    "and start date {startDate} doesn't exist exist! Please, enter another data" +
                    " presss ESC to back Main menu");

                CustomerIdToLocation(ref customerId, ref location);
                carId = CarPlateToCarId();

                Console.WriteLine("Enter your reservation start date");
                startDate = Utility.InputAndValidatDateTime();

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
                Utility.ErrorMessage($"There are no cars in {location}! Please, enter another plate number " +
                    "or presss ESC to back Main menu");
                carId = CarPlateToCarId();
                checkReservExist = carController.FindAvailableCar(carId, location);
            }
        }

        /// <summary>
        /// Chiking customer ID inputs from customers. If it is correct returns customer location
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        public int CheckIfCustomerExist() {
            Console.WriteLine("Enter Client Id to update");
            int customerId = Utility.InputAndValidatInt();

            if(!customerController.FindCustomerById(customerId)){
                Utility.ErrorMessage($"Customer with id: {customerId} doesn't exist");
                Utility.WarningMessage("Try one more! To exit in main menu hit ESC");
                return 0;
            };
            return customerId;
        }

        /// <summary>
        /// Chiking car plate inputs from customers. If it is correct returns true
        /// </summary>
        /// <param name="plate"></param>
        /// <returns></returns>
        public bool CheckIfPlateExist(ref string plate) {
            if (carController.FindCarByPlate(plate) < 1) {
                Utility.ErrorMessage($"Car with number: {plate} doesn't exist");
                Utility.WarningMessage("Try one more! To exit in main menu hit ESC");
                return false;
            }
            return true;
        }

        public void AddCarReservation() {
            Console.Clear();
            reservationController.AddReservation(Forms.ReservationForm(this));

            Utility.SuccessMessage("New Reservation added successful");
        }

        public void AddCustomer() {
            Console.Clear();
            customerController.AddCustomer(Forms.CustomerForm());
            Utility.SuccessMessage("Customer was added successful");
        }

        public void UpdateCustomer() {
            Console.Clear();
            int id = CheckIfCustomerExist();
            while(id < 1) {
                id = CheckIfCustomerExist();
            }

            if (customerController.FindCustomerById(id)) {
                customerController.UpdateCustomer(Forms.CustomerForm(), id);
                Utility.SuccessMessage("Customer was updated successful");
            }
            else {
                Utility.ErrorMessage("Customer was not updated");
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

            Utility.SuccessMessage("Press any key to change reservation");
            Console.ReadKey();

            Utility.WarningMessage("If you want to cancel reservation hit 1 otherwise any button");
            int cancel = Utility.InputAndValidatInt();

            if (cancel == 1) {
                reservationController.CancelReservation(customerId, carId, startDate);
                Utility.SuccessMessage("Reservation was cancelled");
                return;
            }

            reservation = Forms.ReservationForm(this);
            reservationController.UpdateReservation(reservation, customerId, carId, startDate);
            Utility.SuccessMessage("Reservation was updated successful");

        }

        /// <summary>
        /// Every day checking if in the database are unclosed reservations
        /// </summary>
        public void CheckingUnclosedReservations() {
            reservationController.CloseReservation();
        }

    }
}
