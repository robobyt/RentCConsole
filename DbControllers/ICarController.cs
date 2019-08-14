using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    public interface ICarController {
        List<string[]> CarsList(int OrderBy);
        int FindCarByPlate(string plateNumber);
        bool FindAvailableCar(int carId, string location);
        List<string[]> TenMostRecentCars();
        List<string[]> MostRentedCarsByMonth(int order);
    }
}
