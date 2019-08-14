using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole {
    class Forms {
        /// <summary>
        /// Create Customers instance from inputs data
        /// </summary>
        /// <returns></returns>
        public static Customers CustomerForm() {
            Console.Clear();
            string name;
            string location = null;
            DateTime birthDate;

            do{
                Console.WriteLine("Enter your full  name in format 'John Smith':");
                name = Convert.ToString(Console.ReadLine());
            } while(!Utility.CustomerNameValidate(name));

            do {
                Console.WriteLine("Enter Client Birthdate");
                birthDate = Utility.InputAndValidatDateTime();

            } while (!Utility.CheckIfClientIsAdult(birthDate));

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
        public static Reservations ReservationForm(FormController controller) {
            Console.Clear();
            int customerId = 0;
            string location = null;
            DateTime startDate;
            DateTime endDate;

            controller.CustomerIdToLocation(ref customerId, ref location);

            int carId = controller.CarPlateToCarId();

            controller.CheckIfCarAvailableAtLocation(ref carId, ref location);

            do{
                Console.WriteLine("Enter Start date");
                startDate = Utility.InputAndValidatDateTime();
            } while (!Utility.CheckIfCorrectDate(startDate)) ;

             do{
                Console.WriteLine("Enter End date");
                startDate = Utility.InputAndValidatDateTime();

                Console.WriteLine("Input end date:");
                endDate = Utility.InputAndValidatDateTime();
            } while (!Utility.CheckIfEndDateIsCorrect(startDate, endDate)) ;

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
    }
}
