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
        public List<string[]> CarsList(int OrderBy) {
            List<string[]> cars = new List<string[]>();

            string sqlExpression =
                @"SELECT CarID, Plate, Manufacturer, Model, PricePerDay, Location FROM Cars WHERE IsBusy = 0
                  ORDER BY 
                    CASE WHEN @OrderBy = 0
                    THEN CarID END ASC,
                    CASE when @OrderBy = 1
                    THEN CarID END DESC,
                    CASE WHEN @OrderBy = 2
                    THEN Plate END ASC,
                    CASE WHEN @OrderBy = 3
                    THEN Plate END DESC,
                    CASE WHEN @OrderBy = 4
                    THEN Manufacturer END ASC,
                    CASE WHEN @OrderBy = 5
                    THEN Manufacturer END DESC,
                    CASE WHEN @OrderBy = 6
                    THEN Model END ASC,
                    CASE WHEN @OrderBy = 7
                    THEN Model END DESC,
                    CASE WHEN @OrderBy = 8
                    THEN PricePerDay END ASC,
                    CASE WHEN @OrderBy = 9
                    THEN PricePerDay END DESC,
                    CASE WHEN @OrderBy = 10
                    THEN Location END ASC,
                    CASE WHEN @OrderBy = 11
                    THEN Location END DESC";

            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {

                    cars.Add(new string[] { "CarID:", "Plate:", "Manufacturer:", " Model:", "PricePerDay:", "Location:" });
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
                    //Console.WriteLine("Car with plate number {0} doesn't exist.", plateNumber);
                    return 0;
                }
                else
                    //Console.WriteLine("Car with plate number {0} exist.", plateNumber);

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
                    //Console.WriteLine("There isn't available car in {0}.", location);
                    return false;
                }
                //Console.WriteLine("There is available car in {0}.", location);

                return true;
            }
        }
       
        public List<string[]> MostRentedCarsByMonth(int OrderBy) {
            List<string[]> cars = new List<string[]>();

            string sqlExpression =
                 @"SELECT COUNT(*), C.CarID, C.Plate, C.Manufacturer, C.Model, C.PricePerDay, C.Location 
                    FROM Cars AS C JOIN Reservations AS R 
                    ON R.CarID = C.CarID
                    WHERE R.StartDate >= @Month
                    GROUP BY C.CarID, C.Plate, C.Manufacturer, C.Model, C.PricePerDay, C.Location
                    ORDER BY 
                    CASE WHEN @OrderBy = 0
                    THEN COUNT(*) END ASC,
                    CASE when @OrderBy = 1
                    THEN COUNT(*) END DESC";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@Month", DateTime.Now.AddMonths(-1));
                cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    cars.Add(new string[] { "Ordered times:", "CarID:", "Plate:", "Manufacturer:", "Model:", "PricePerDay:", "Location:"});
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

        public List<string[]> TenMostRecentCars() {
            List<string[]> cars = new List<string[]>();

            string sqlExpression =
                @"SELECT TOP 10 C.CarID, C.Plate, C.Manufacturer, C.Model, C.PricePerDay, C.Location,
                    R.StartDate, R.EndDate
                    FROM Cars AS C JOIN Reservations AS R 
                    ON R.CarID = C.CarID
                    WHERE R.StartDate >= @Month";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@Month", DateTime.Now.AddMonths(-1));
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    cars.Add(new string[] { "CarID:", "Plate:", "Manufacturer:", "Model:", "PricePerDay:", "Location:", "StartDate:", "EndDate:" });
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
    }
}
