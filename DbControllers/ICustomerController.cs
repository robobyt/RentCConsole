using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    public interface ICustomerController {
        List<string[]> CustomerList(int parameter);
        void AddCustomer(Customers customer);
        void UpdateCustomer(Customers customer, int customerID);
        string FindCustomerLocationById(int customerID);
        bool FindCustomerById(int customerID);
        List<string[]> GoldAndSilverCustomers(int count);
    }
}
