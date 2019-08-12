using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    interface ICarController {
        List<string[]> CarsList();
        int FindCarByPlate(string plateNumber);
        bool FindAvailableCar(int carId, string location);
    }
}
