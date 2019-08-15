using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RentCConsole {
    public static class Utility {
        public static DateTime InputAndValidatDateTime() {
            DateTime date = DateTime.UtcNow;
            while (!DateTime.TryParse(Console.ReadLine(), out date)) {
                WarningMessage("Please enter date in format: dd.mm.year");
            }
            return date;
        }

        public static int InputAndValidatInt() {
            int number = 0;
            while (!int.TryParse(Console.ReadLine(), out number)) {
                WarningMessage("Please enter positive number");
            }
            return number;
        }

        public static bool CustomerNameValidate(string name) {
            Regex regex = new Regex(@"[A-Z][a-z]+\s[A-Z][a-z]+");
            if (string.IsNullOrEmpty(name)) {
                Console.WriteLine("Name field can't be empty");
                return false;
            }

            if (!regex.IsMatch(name)) {
                return false;
            } 
                
            return true;
        }

        public static bool  CheckIfEndDateIsCorrect(DateTime startDate, DateTime endDate) {
            if(DateTime.Compare(endDate, startDate) < 0) {
                WarningMessage("End date can't be earlier then start date. Input start date:");
                return false;
            }
            return true;
        }

        // need to fix it
        public static bool CheckIfClientIsAdult(DateTime birthDate) {
             if(DateTime.Compare(DateTime.Now, birthDate.AddYears(18)) < 0) {
                WarningMessage("Enter Client Birthdate. Client has to be turn 18 yars. ");
                return false;
            }

            return true;
        }

        public static bool CheckIfCorrectDate(DateTime startDate) {
              if(startDate.Day < DateTime.Now.Day) {
                WarningMessage("Please, set date not earlier then today");
                return false;
            }
            return true;
        }

        public static void WarningMessage(string str) {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
        }

        public static void ErrorMessage(string str) {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
        }

        public static void SuccessMessage(string str) {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(str);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
        }
    }
}
