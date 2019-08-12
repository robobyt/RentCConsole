using RentCConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.DbControllers {
    interface IReservationController {
        List<string[]> ReservationsList();
        void CloseReservation();
        bool FindReservationByKeys(int CustomerId, int CarId, DateTime startDate);
        void UpdateReservation(Reservations reservation, int oldCustomerID, int oldCarID, DateTime oldStartDate);
        void CancelReservation(int CustomerID, int CarID, DateTime StartDate);
        void AddReservation(Reservations reservation);

    }
}
