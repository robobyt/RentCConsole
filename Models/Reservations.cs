using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole.Models {
    class Reservations {
        public Customers customer { get; set; }
        public Cars car { get; set; }
        public Coupons CouponCode { get; set; }
        public ReservationStatuses reservationStatuses { get; set; }
        public int ReservStatsID { get; set; }
        public int CustomerID { get; set; }
        public int CarID { get; set; }
        public string CarPlate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }

    }
}
