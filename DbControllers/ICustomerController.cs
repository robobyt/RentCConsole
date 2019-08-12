using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    interface ICustomerController {
        List<string[]> CustomerList();
        void AddCustomer(Customers customer);
        void UpdateCustomer(Customers customer, int customerID);
        string FindCustomerLocationById(int customerID);
        bool FindCustomerById(int customerID);
    }
}
