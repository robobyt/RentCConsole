using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string CustomerNameValidate(string name) {
            Regex regex = new Regex(@"[A-Z][a-z]+\s[A-Z][a-z]+");
            while (!regex.IsMatch(name)) {
                Console.WriteLine("Enter your full  name in format 'John Smith':");
                name = Convert.ToString(Console.ReadLine());
                if (string.IsNullOrEmpty(name)){
                    Console.WriteLine("Name field can't be empty");
                    continue;
                    }
                }
            return name;
        }

        //TODO Use TimeSpan
        public static void  CheckIfEndDateIsCorrect(ref DateTime startDate, ref DateTime endDate) {
            while (DateTime.Compare(endDate, startDate) < 0) {
                Console.WriteLine("End date can't be earlier then start date. Input start date:");
                startDate = InputAndValidatDateTime();
                Console.WriteLine("Input end date:");
                endDate = InputAndValidatDateTime();
            }
        }

        public static DateTime CheckIfClientIsAdult(DateTime birthDate) {
            while (DateTime.Now.Day > birthDate.AddYears(18).Day) {
                Console.WriteLine("Client has to be turn 18 yars. Enter Client Birthdate");
                birthDate = Utility.InputAndValidatDateTime();
            }

            return birthDate;
        }

        public static DateTime CheckIfCorrectDate(DateTime startDate) {
            while (startDate.Day < DateTime.Now.Day) {
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
