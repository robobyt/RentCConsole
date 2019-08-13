using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    public class CarController : ICarController{
        private SqlConnection connection;

        public CarController(SqlConnection con) {
            connection = con;
        }

        /// <summary>
        /// Returns List of string arrays of all available cars. It means all cars those were not booked
        /// </summary>
        public List<string[]> CarsList() {
            List<string[]> cars = new List<string[]>();

            using (SqlCommand cmd = new SqlCommand("SELECT CarID, Plate, Manufacturer, Model, PricePerDay FROM Cars WHERE IsBusy = 0", connection)) {
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {

                    cars.Add(new string[] { "CarID:", "Plate:", "Manufacturer:", " Model:", "PricePerDay:" });
                    while (reader.Read()) {
                        string[] row = new string[reader.FieldCount];
                        for (int i = 0; i < row.Length; i++) {
                            row[i] = reader[i].ToString();
                        }
                        cars.Add(row);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return cars;
        }

        /// <summary>
        /// Try to find the car by plate number. If car exists method returns CarID
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public int FindCarByPlate(string plateNumber) {
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
        public bool FindAvailableCar(int carId, string location) {
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
