using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole {
    class CarWebService {
        /// <summary>
        /// We open client for web service that consumes data from database with the available cars
        /// </summary>
        public List<string[]> WebServiceCarsList(int orderBy) {
            Console.Clear();

            var client = new RentWebService.RentCWebServiceSoapClient();
            var arr = client.Cars(orderBy);
            client.Close();

            List<string[]> cars = new List<string[]>();
            foreach (var item in arr) {
                string[] row = new string[item.Count];
                for (int i = 0; i < item.Count; i++) {
                    row[i] = item[i];
                }
                cars.Add(row);
            }

            return cars;
        }
    }
}
