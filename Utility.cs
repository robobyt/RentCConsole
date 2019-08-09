using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCConsole {
    static class Utility {
        public static DateTime InputAndValidatDateTime() {
            DateTime date = DateTime.UtcNow;
            while (!DateTime.TryParse(Console.ReadLine(), out date)) {
                Console.WriteLine("Please enter date in format: dd.mm.year");
            }
            return date;
        }

        public static int InputAndValidatInt() {
            int number = 0;
            while (!int.TryParse(Console.ReadLine(), out number)) {
                Console.WriteLine("Please enter positive number");
            }
            return number;
        }

        public static DateTime CheckIfEndDateIsCorrect(DateTime startDate, DateTime endDate) {
            while (endDate < startDate) {
                Console.WriteLine("End date can't be earlier then start date. Input start date:");
                startDate = InputAndValidatDateTime();
                Console.WriteLine("Input end date:");
                endDate = InputAndValidatDateTime();
            }

            return startDate;
        }

        public static DateTime CheckIfClientIsAdult(DateTime birthDate) {
            while (DateTime.Now.Year - birthDate.Year < 18) {
                birthDate = Utility.InputAndValidatDateTime();
                Console.WriteLine("Client has to be turn 18 yars");
            }

            return birthDate;
        }

        public static DateTime CheckIfCorrectDate(DateTime startDate) {
            while (startDate < DateTime.Now) {
                Console.WriteLine("Please, set date not earlier then today");
                startDate = Utility.InputAndValidatDateTime();
            }

            return startDate;
        }

        public static void WarningMessage(string str) {

        }

        public static void ErrorMessage(string str) {

        }

        public static void SuccessMessage(string str) {

        }
    }
}
