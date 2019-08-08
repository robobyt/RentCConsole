using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    class ReservationController {
        private SqlConnection connection;

        public ReservationController(SqlConnection con) {
            connection = con;
        }

        public void ReservationsList() {
            string sqlExpression =
                "SELECT Cars.Plate, Reservations.CustomerID, Reservations.StartDate, " +
                "Reservations.EndDate, Reservations.Location" +
                " FROM Reservations JOIN Cars ON Reservations.CarID = Cars.CarID";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    Console.WriteLine("Car Plate | ClientID | StartDate | EndDate | Location");
                    while (reader.Read()) {
                        Console.WriteLine("{0} | {1} | {2} | {3} | {4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
                    }
                    reader.Close();
                }
                connection.Close();
            }
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
                    Console.WriteLine("Reservation with ID {0} exist. You can update data", reservationPK);
                    return true;
                }
                else {
                    Console.WriteLine("Reservation with ID {0} doesn't exist.", reservationPK);
                    return false;
                }
            }
        }

        /// <summary>
        /// After find query by using FindReservationByKeys we can update it with new instance of model.
        /// With instance we also push through the parameters ID's in order to find query.
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="oldCustomerID"></param>
        /// <param name="oldCarID"></param>
        /// <param name="oldStartDate"></param>
        internal void UpdateReservation(Reservations reservation, int oldCustomerID, int oldCarID, DateTime oldStartDate) {
            string sqlExpression =
                @"UPDATE Reservations SET CarID=@CarID, CustomerID=@CustomerID, 
                 StartDate=@StartDate, EndDate=@EndDate,
                 ReservStatsID=@ReservStatsID, Location=@Location
                 WHERE CustomerID=@oldCustomerID AND CarID=@oldCarID AND StartDate=@oldStartDate";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@oldCarID", oldCarID);
                cmd.Parameters.AddWithValue("@oldCustomerID", oldCustomerID);
                cmd.Parameters.AddWithValue("@oldStartDate", oldStartDate);
                cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
                cmd.Parameters.AddWithValue("@CustomerID", reservation.CustomerID);
                cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate);
                cmd.Parameters.AddWithValue("@ReservStatsID", reservation.ReservStatsID);
                cmd.Parameters.AddWithValue("@Location", reservation.Location);

                connection.Open();
                int number = cmd.ExecuteNonQuery();
                Console.WriteLine("{0} Reservation was updated", number);
                connection.Close();
            }
        }

        internal void AddReservation(Reservations reservation) {
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
                int number = cmd.ExecuteNonQuery();
                Console.WriteLine("{0} Reservations was added ", number);
                connection.Close();
            }
        }
    }
}
