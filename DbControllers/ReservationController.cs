using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    class ReservationController : IReservationController {
        private SqlConnection connection;

        public ReservationController(SqlConnection con) {
            connection = con;
        }

        /// <summary>
        /// Returns List of string arrays of all reservations 
        /// </summary>
        public List<string[]> ReservationsList(int OrderBy) {
            List<string[]> reservations = new List<string[]>();

            string sqlExpression =
                @"SELECT Cars.Plate, Reservations.CustomerID, Reservations.StartDate,  
                Reservations.EndDate, Reservations.Location 
                FROM Reservations JOIN Cars ON Reservations.CarID = Cars.CarID
                  ORDER BY 
                    CASE WHEN @OrderBy = 0
                    THEN Cars.Plate END ASC,
                    CASE when @OrderBy = 1
                    THEN Cars.Plate END DESC,
                    CASE WHEN @OrderBy = 2
                    THEN Reservations.CustomerID END ASC,
                    CASE WHEN @OrderBy = 3
                    THEN Reservations.CustomerID END DESC,
                    CASE WHEN @OrderBy = 4
                    THEN Reservations.StartDate END ASC,
                    CASE WHEN @OrderBy = 5
                    THEN Reservations.StartDate END DESC,
                    CASE WHEN @OrderBy = 6
                    THEN Reservations.EndDate END ASC,
                    CASE WHEN @OrderBy = 7
                    THEN Reservations.EndDate END DESC,
                    CASE WHEN @OrderBy = 8
                    THEN Reservations.Location END ASC,
                    CASE WHEN @OrderBy = 9
                    THEN Reservations.Location END DESC";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    reservations.Add(new string[] { "Car Plate:", "ClientID:", "StartDate:", "EndDate:", "Location:" });
                    while (reader.Read()) {
                        string[] row = new string[reader.FieldCount];
                        for (int i = 0; i < row.Length; i++) {
                            row[i] = reader[i].ToString();
                        }
                        reservations.Add(row);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return reservations;
        }

        public void CloseReservation() {
            string sql = "SELECT COUNT(*) FROM Reservations where date(EndDate) < date(now())"; ;
        }

        /// <summary>
        /// In order to update Reservation we need to find query by CustomerId and CarId as these ID's 
        /// are Reservations_pk created through the PRIMARY KEY CLUSTERED. One customer isn't
        /// allowed to book same car in one momement 
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="CarId"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public bool FindReservationByKeys(int CustomerId, int CarId, DateTime startDate) {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Reservations" +
                " WHERE CustomerID=@CustomerID AND CarID=@CarID AND StartDate=@StartDate", connection)) {
                connection.Open();
                cmd.Parameters.AddWithValue("@CarID", CarId);
                cmd.Parameters.AddWithValue("@CustomerID", CustomerId);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                int reservationPK = (int)cmd.ExecuteScalar();
                connection.Close();

                if (reservationPK > 0) {
                    //Console.WriteLine("Reservation with ID {0} exist. You can update data", reservationPK);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// After we find the query through the FindReservationByKeys method we can update it with the new instance of model.
        /// With instance methog excepts the parameters ID's to find query.
        /// 
        /// Next we have to check if car was changed. If so:
        /// Old car set available and new car set busy
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="oldCustomerID"></param>
        /// <param name="oldCarID"></param>
        /// <param name="oldStartDate"></param>
        public void UpdateReservation(Reservations reservation, int oldCustomerID, int oldCarID, DateTime oldStartDate) {
            string firstExpression =
                @"UPDATE Reservations SET CarID=@CarID, CustomerID=@CustomerID, 
                 StartDate=@StartDate, EndDate=@EndDate, Location=@Location
                 WHERE CustomerID=@oldCustomerID AND CarID=@oldCarID AND StartDate=@oldStartDate";
            using (SqlCommand cmd = new SqlCommand(firstExpression, connection)) {
                cmd.Parameters.AddWithValue("@oldCarID", oldCarID);
                cmd.Parameters.AddWithValue("@oldCustomerID", oldCustomerID);
                cmd.Parameters.AddWithValue("@oldStartDate", oldStartDate);
                cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
                cmd.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                cmd.Parameters.AddWithValue("@Location", reservation.Location);

                connection.Open();
                cmd.ExecuteNonQuery();
                //Console.WriteLine("Reservation was updated");
            }

            if(oldCarID == reservation.CarID) {
                connection.Close();
                return;
            }

            string secondExpression =
                 @"UPDATE Cars SET Cars.IsBusy=@IsBusy WHERE CarID=@oldCarID";
            using (SqlCommand cmd = new SqlCommand(secondExpression, connection)) {
                cmd.Parameters.AddWithValue("@IsBusy", false);
                cmd.ExecuteNonQuery();
                //Console.WriteLine("{0} Car with ID is available now", oldCarID);
            }

            string thirdExpression =
                @"UPDATE Cars SET Cars.IsBusy=@IsBusy WHERE CarID=@CarID";
            using (SqlCommand cmd = new SqlCommand(thirdExpression, connection)) {
                cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
                cmd.Parameters.AddWithValue("@IsBusy", true);
                cmd.ExecuteNonQuery();
                //Console.WriteLine("{0} Car with ID is reserved", reservation.CarID);
                connection.Close();
            }

        }

        /// <summary>
        /// When customer wants to cancel the reservation we call CancelReservation we parameters of ID's.
        /// In reservation we set ReservStatsID to cancel and set the Car is available.
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="CarID"></param>
        /// <param name="StartDate"></param>
        public void CancelReservation(int CustomerID, int CarID, DateTime StartDate) {
            string firstExpression =
                @"UPDATE Reservations SET ReservStatsID=@ReservStatsID WHERE CustomerID=@CustomerID AND CarID=@CarID AND StartDate=@StartDate";
            using (SqlCommand cmd = new SqlCommand(firstExpression, connection)) {
                cmd.Parameters.AddWithValue("@CarID", CarID);
                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                cmd.Parameters.AddWithValue("@StartDate", StartDate);
                cmd.Parameters.AddWithValue("@ReservStatsID", 3);

                connection.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Reservation was updated");
            }

            string secondExpression =
                @"UPDATE Cars SET Cars.IsBusy=@IsBusy WHERE CarID=@CarID";
            using (SqlCommand cmd = new SqlCommand(secondExpression, connection)) {
                cmd.Parameters.AddWithValue("@CarID", CarID);
                cmd.Parameters.AddWithValue("@IsBusy", false);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Car with ID {0} is available now", CarID);
                connection.Close();
            }
        }

        /// <summary>
        /// The method accepts Reservations instance and adds a new кусщкв to the database. 
        /// Set car to Busy. So, this car will not be available for the next customer until the end date.
        /// </summary>
        /// <param name="reservation"></param>
        public void AddReservation(Reservations reservation) {
            string sqlExpression =
                @"INSERT INTO Reservations (CarID, CustomerID, StartDate, 
                            EndDate, ReservStatsID, Location)
                            VALUES (@CarID, @CustomerID, @StartDate, 
                            @EndDate, @ReservStatsID, @Location)";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
                cmd.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                cmd.Parameters.AddWithValue("@ReservStatsID", 1); // means Status = Open. 
                cmd.Parameters.AddWithValue("@Location", reservation.Location);

                connection.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Reservations was added");
            }

            string secondExpression =
                @"UPDATE Cars SET Cars.IsBusy=@IsBusy WHERE CarID=@CarID";
            using (SqlCommand cmd = new SqlCommand(secondExpression, connection)) {
                cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
                cmd.Parameters.AddWithValue("@IsBusy", true);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Car with ID {0} is reserved", reservation.CarID);
                connection.Close();
            }
        }
    }
}
