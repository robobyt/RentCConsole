using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    class CustomerController {
        private SqlConnection connection;

        public CustomerController(SqlConnection con) {
            connection = con;
        }

        public void CustomerList() {
            using (SqlCommand cmd = new SqlCommand("SELECT CustomerID, Name, BirthDate FROM Customers", connection)) {
                connection.Open();
                cmd.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (SqlDataReader reader = cmd.ExecuteReader()) {
                    Console.WriteLine("CustomerID: |  Name: | BirthDate: |");
                    while (reader.Read()) {
                        Console.WriteLine("{0}         | {1}        | {2}", reader[0], reader[1], reader[2]);
                    }

                    reader.Close();
                }
                connection.Close();
            }
 
        }

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
                Console.WriteLine("{0} Customers was added: ", number);
                connection.Close();
            }
       
        }

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
                Console.WriteLine("{0} Customers was updated", number);
                connection.Close();
            }

        }

        public bool FindCustomerById(int customerID) {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CustomerID = @customerID", connection)) {
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
        }
    }
}
