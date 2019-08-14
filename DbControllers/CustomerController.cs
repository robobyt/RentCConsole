using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    class CustomerController : ICustomerController {
        private SqlConnection connection;

        public CustomerController(SqlConnection con) {
            connection = con;
        }

        /// <summary>
        /// Returns List of string arrays of all customers
        /// </summary>
        public List<string[]> CustomerList(int OrderBy) {

            List<string[]> customers = new List<string[]>();
            string sqlExpression =
                @"SELECT CustomerID, Name, BirthDate, Location FROM Customers 
                  ORDER BY 
                    CASE WHEN @OrderBy = 0
                    THEN CustomerID END ASC,
                    CASE when @OrderBy = 1
                    THEN CustomerID END DESC,
                    CASE WHEN @OrderBy = 2
                    THEN Name END ASC,
                    CASE WHEN @OrderBy = 3
                    THEN Name END DESC,
                    CASE WHEN @OrderBy = 4
                    THEN BirthDate END ASC,
                    CASE WHEN @OrderBy = 5
                    THEN BirthDate END DESC,
                    CASE WHEN @OrderBy = 6
                    THEN Location END ASC,
                    CASE WHEN @OrderBy = 7
                    THEN Location END DESC";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    customers.Add(new string[] { "CustomerID:", "Name:", "BirthDate:", "Location:" });
                    while (reader.Read()) {
                        string[] row = new string[reader.FieldCount];
                        for (int i = 0; i < row.Length; i++) {
                            row[i] = reader[i].ToString();
                        }
                        customers.Add(row);
                    }
                    reader.Close();
                }
                connection.Close();
            }
            return customers;
        }

        /// <summary>
        /// The method accepts Customers instance and adds a new query to the database. 
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customers customer) {

            string sqlExpression =
                @"INSERT INTO Customers (Name, BirthDate, Location)
                            VALUES (@Name, @BirthDate, @Location)";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
                cmd.Parameters.AddWithValue("@Location", customer.Location);

                connection.Open();
                int number = cmd.ExecuteNonQuery();
                //Console.WriteLine("{0} Customers was added: ", number);
                connection.Close();
            }
       
        }

        /// <summary>
        /// The method accepts id  of existing Customers record and change it to Customers instance
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="customerID"></param>
        public void UpdateCustomer(Customers customer, int customerID) {

            string sqlExpression =
                @"UPDATE Customers SET Name=@Name, BirthDate=@BirthDate, Location=@Location WHERE CustomerID = @customerID";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@customerID", customerID);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
                cmd.Parameters.AddWithValue("@Location", customer.Location);

                connection.Open();
                int number = cmd.ExecuteNonQuery();
                //Console.WriteLine("{0} Customers was updated", number);
                connection.Close();
            }

        }

        /// <summary>
        /// If we want to update Customers record we have to be sure that this record exists. 
        /// Method returns true if CustomerID was finded
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool FindCustomerById(int customerID) {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CustomerID = @customerID", connection)) {
                connection.Open();
                cmd.Parameters.AddWithValue("@customerID", customerID);
                int customerExist = (int)cmd.ExecuteScalar();
                connection.Close();

                if (customerExist > 0) {
                    //Console.WriteLine("Customer with ID {0} exist. You can update data", customerID);
                    return true;
                }
                //Console.WriteLine("Customer with ID {0} doesn't exist.", customerID);
                return false;
            }
        }

        /// <summary>
        /// Method returns customer location if record was found by Customer ID.
        /// We need to know customer location for compare it with the car location
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public string FindCustomerLocationById(int customerID) {
            using (SqlCommand cmd = new SqlCommand("SELECT Location FROM Customers WHERE CustomerID = @customerID", connection)) {
                connection.Open();
                cmd.Parameters.AddWithValue("@customerID", customerID);
                object customerLocation = cmd.ExecuteScalar();
                connection.Close();

                if (customerLocation == null) {
                    //Console.WriteLine("Customer with ID {0} doesn't exist.", customerID);
                    return null;
                }
                //Console.WriteLine("Customer with ID {0} exist. You can update data", customerID);
                return (string)customerLocation;
            }
        }

        public List<string[]> GoldAndSilverCustomers(int count) {
            List<string[]> cars = new List<string[]>();

            string sqlExpression =
                @"SELECT COUNT(*), C.CustomerID, C.Name, C.BirthDate, C.Location
                    FROM Customers AS C JOIN Reservations AS R 
                    ON R.CustomerID = C.CustomerID
                    WHERE R.StartDate >= @Month
                    GROUP BY C.CustomerID, C.Name, C.BirthDate, C.Location
                    HAVING COUNT(R.CustomerId) >= @Count
                    ORDER BY COUNT(*) DESC";
            using (SqlCommand cmd = new SqlCommand(sqlExpression, connection)) {
                cmd.Parameters.AddWithValue("@Month", DateTime.Now.AddMonths(-1));
                cmd.Parameters.AddWithValue("@Count", count);

                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    cars.Add(new string[] { "Ordered times:", "CustomerID:", "Name:", "BirthDate:", "Location:"});
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
