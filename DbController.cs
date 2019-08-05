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

        internal void CustomersList() {
            SqlCommand cmd = new SqlCommand("SELECT CostumerID, Name, BirthDate FROM Customers", connection);
            connection.Open();
            cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("CostomerID: |  Name: | BirthDate: |");
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

        internal void RentsList() {
            string sqlExpression =
                "SELECT Cars.Plate, Reservations.CostumerID, Reservations.StartDate, " +
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
                @"UPDATE Customers SET Name=@Name, BirthDate=@BirthDate, Location=@Location WHERE CostumerID = @customerID";
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
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CostumerID = @customerID", connection);
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

        internal int FindCarByPlate(string plateNumber) {
            SqlCommand cmd = new SqlCommand("SELECT CarID FROM Cars WHERE Plate = @plate", connection);
            connection.Open();
            cmd.Parameters.AddWithValue("@plate", plateNumber);

            int carID = (int)cmd.ExecuteScalar();

            if (carID > 0) 
                Console.WriteLine("Car with plate number {0} exist.", plateNumber);
            else  
                Console.WriteLine("Car with plate number {0} doesn't exist.", plateNumber);

            connection.Close();
            return carID;
        }

        internal void AddCarRent(Reservations reservation) {

            string sqlExpression =
                @"INSERT INTO Reservations (Cars.CarID, Reservations.CostumerID, Reservations.StartDate, Reservations.EndDate, Reservations.ReservStatsID, Reservations.Location)
                            VALUES (@CarID, @CostumerID, @StartDate, @EndDate, @ReservStatsID, @Location)";
            SqlCommand cmd = new SqlCommand(sqlExpression, connection);

            cmd.Parameters.AddWithValue("@CarID", reservation.CarID);
            cmd.Parameters.AddWithValue("@CostumerID", reservation.CustomerID);
            cmd.Parameters.AddWithValue("@StartDate", reservation.StartDate); // Is it bad idea set Start date automaticaly with DateNow?
            cmd.Parameters.AddWithValue("@EndDate", reservation.EndDate); //  why do we need set EndDate when we open reservation?
            cmd.Parameters.AddWithValue("@ReservStatsID", 1); // means Status = Open. 
            cmd.Parameters.AddWithValue("@Location", reservation.Location);

            connection.Open();
            int number = cmd.ExecuteNonQuery();
            Console.WriteLine("{0} Reservations was added ", number);
            connection.Close();
        }


    }
}
