using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.Models {
    public class Customers {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Location { get; set; }
    }
}
