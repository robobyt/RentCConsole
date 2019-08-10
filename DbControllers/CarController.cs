using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    class CarController {
        private SqlConnection connection;

        internal CarController(SqlConnection con) {
            connection = con;
        }

        /// <summary>
        /// Display all available cars. It means all cars those were not booked
        /// </summary>
        internal void CarsList() {
            using (SqlCommand cmd = new SqlCommand("SELECT CarID, Plate, Manufacturer, Model, PricePerDay FROM Cars WHERE IsBusy = 0", connection)) {
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    Console.WriteLine("CarID: |  Plate: | Manufacturer: | Model: | PricePerDay: |");
                    while (reader.Read()) {
                        Console.WriteLine("{0}         | {1}        | {2}      | {3}     | {4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
                    }
                    reader.Close();
                }
                connection.Close();
            }
                
        }

        /// <summary>
        /// Try to find the car by plate number. If car exists method returns CarID
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        internal int FindCarByPlate(string plateNumber) {
            using (SqlCommand cmd = new SqlCommand("SELECT CarID FROM Cars WHERE Plate = @plate", connection)) {
                connection.Open();
                cmd.Parameters.AddWithValue("@plate", plateNumber);

                object carID = cmd.ExecuteScalar();
                connection.Close();

                if (carID == null) {
                    Console.WriteLine("Car with plate number {0} doesn't exist.", plateNumber);
                    return 0;
                }
                else
                    Console.WriteLine("Car with plate number {0} exist.", plateNumber);

                return (int)carID;
            }
 
        }

        /// <summary>
        /// When a customer wants to create a new reservation we have to be sure
        /// that the car is available in the city where the customer is.
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        internal bool FindAvailableCar(int carId, string location) {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cars " +
                "WHERE CarID = @CarId AND IsBusy = 0 AND Location = @location", connection)) {
                connection.Open();
                cmd.Parameters.AddWithValue("@CarID", carId);
                cmd.Parameters.AddWithValue("@location", location);

                object carID = cmd.ExecuteScalar();
                connection.Close();

                if (carID == null || (int)carID == 0) {
                    Console.WriteLine("There isn't available car in {0}.", location);
                    return false;
                }
                else
                    Console.WriteLine("There is available car in {0}.", location);

                return true;
            }
        }
    }
}
