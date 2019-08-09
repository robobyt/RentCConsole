using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole {
    class DbController {
        private SqlConnection connection;

        internal DbController(SqlConnection con) {
            connection = con;
        }

        // TODO Change to using in order to don't miss Dispose method
        internal void CustomersList() {
            SqlCommand cmd = new SqlCommand("SELECT CustomerID, Name, BirthDate FROM Customers", connection);
            connection.Open();
            cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("CustomerID: |  Name: | BirthDate: |");
            while (reader.Read()) {
                Console.WriteLine("{0}         | {1}        | {2}", reader[0], reader[1], reader[2]);
            }

            reader.Close();
            connection.Close();
        }

        internal void CarsList() {
            SqlCommand cmd = new SqlCommand("SELECT CarID, Plate, Manufacturer, Model, PricePerDay FROM Cars", connection);
            connection.Open();
            cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("CarID: |  Plate: | Manufacturer: | Model: | PricePerDay: |");
            while (reader.Read()) {
                Console.WriteLine("{0}         | {1}        | {2}      | {3}     | {4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
            }

            reader.Close();
            connection.Close();
        }

        internal void ReservationsList() {
            string sqlExpression =
                "SELECT Cars.Plate, Reservations.CustomerID, Reservations.StartDate, " +
                "Reservations.EndDate, Reservations.Location" +
                " FROM Reservations JOIN Cars ON Reservations.CarID = Cars.CarID";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);
            connection.Open();
            cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("Car Plate | ClientID | StartDate | EndDate | Location");
            while (reader.Read()) {
                Console.WriteLine("{0} | {1} | {2} | {3} | {4}", reader[0], reader[1], reader[2], reader[3], reader[4]);
            }

            reader.Close();
            connection.Close();
        }

        internal void AddCustomer(Customers customer) {

            string sqlExpression = 
                @"INSERT INTO Customers (Name, BirthDate, Location)
                            VALUES (@Name, @BirthDate, @Location)";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);

            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
            cmd.Parameters.AddWithValue("@Location", customer.Location);

            connection.Open();
            int number = cmd.ExecuteNonQuery();
            Console.WriteLine("{0} Customers was added: ", number);
            connection.Close();
        }

        internal void UpdateCustomer(Customers customer, int customerID) {
      
            string sqlExpression =
                @"UPDATE Customers SET Name=@Name, BirthDate=@BirthDate, Location=@Location WHERE CustomerID = @customerID";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);

            cmd.Parameters.AddWithValue("@customerID", customerID);
            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
            cmd.Parameters.AddWithValue("@Location", customer.Location);

            connection.Open();
            int number = cmd.ExecuteNonQuery();
            Console.WriteLine("{0} Customers was updated", number);
            connection.Close();
        }

        internal bool FindCustomerById(int customerID) {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CustomerID = @customerID", connection);
            connection.Open();
            cmd.Parameters.AddWithValue("@customerID", customerID);
            int customerExist = (int)cmd.ExecuteScalar();
            connection.Close();

            if (customerExist > 0) {
                Console.WriteLine("Customer with ID {0} exist. You can update data", customerID);
                return true;
            }

            Console.WriteLine("Customer with ID {0} doesn't exist.", customerID);
            return false;
        }

        internal bool FindReservationByKeys(int CustomerId, int CarId, DateTime startDate) {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Reservations" +
                " WHERE CustomerID=@CustomerID AND CarID=@CarID AND StartDate=@StartDate", connection);
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

        internal void UpdateReservation(Reservations reservation, int oldCustomerID, int oldCarID, DateTime oldStartDate) {
            string sqlExpression =
                @"UPDATE Reservations SET CarID=@CarID, CustomerID=@CustomerID, 
                 StartDate=@StartDate, EndDate=@EndDate,
                 ReservStatsID=@ReservStatsID, Location=@Location
                 WHERE CustomerID=@oldCustomerID AND CarID=@oldCarID AND StartDate=@oldStartDate";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);

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


        internal int FindCarByPlate(string plateNumber) {
            SqlCommand cmd = new SqlCommand("SELECT CarID FROM Cars WHERE Plate = @plate", connection);
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

        internal void AddReservation(Reservations reservation) {

            string sqlExpression =
                @"INSERT INTO Reservations (CarID, CustomerID, StartDate, 
                            EndDate, ReservStatsID, Location)
                            VALUES (@CarID, @CustomerID, @StartDate, 
                            @EndDate, @ReservStatsID, @Location)";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);

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
