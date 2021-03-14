using NavLibrary;
using NavLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NavFlightReservation
{
    class Program
    {
        static CsvFileWriter csvFileWriter;
        static CsvFileReader csvFileReader;

        static void Main(string[] args)
        {
        reservation:

            Console.Clear();
            Console.WriteLine("NAVITAIRE FLIGHT RESERVATION \n TYPE AIRLINE CODE:");
            var airlineCode = Console.ReadLine();

            if (string.IsNullOrEmpty(airlineCode))
            {
                goto reservation;
            }

        searchFlightNumber:

            Console.WriteLine("TYPE FLIGHT NUMBER:");
            if (!int.TryParse(Console.ReadLine(), out int flightNumber))
            {
                goto searchFlightNumber;
            }

            var flights = SearchByCodeAndFlightNumber(airlineCode, flightNumber);

            if (flights.Count <= 0)
            {
                Console.WriteLine("No flights found.");
            }

        setPassengers:

            Console.WriteLine("HOW MANY PASSENGERS ARE THERE? (5 MAXIMUM PASSENGERS)");

            if (!int.TryParse(Console.ReadLine(), out int passengerCount))
            {
                goto setPassengers;
            }

            List<Passenger> passengers;

            for (int i = 0; i < passengerCount; i++)
            {
                Console.WriteLine($"ENTER PASSENGER {i} INFORMATION");

            setFirstName:
                Console.WriteLine("First Name: ");

                var firstName = Console.ReadLine();

                if (string.IsNullOrEmpty(firstName))
                {
                    goto setFirstName;
                }

            setlastName:
                Console.WriteLine("Last Name: ");

                var lastName = Console.ReadLine();

                if (string.IsNullOrEmpty(lastName))
                {
                    goto setlastName;
                }

            setBirthdate:
                Console.WriteLine("Birth Date (MM/DD/YYYY): ");
                Console.WriteLine("Month: (01-12)");

                if (!int.TryParse(Console.ReadLine(), out int month) ||
                    month < 1 ||
                    month > 12)
                {
                    goto setBirthdate;
                }

                if (!int.TryParse(Console.ReadLine(), out int day) ||
                 month < 1 ||
                 month > 31)
                {
                    goto setBirthdate;
                }


                if (!int.TryParse(Console.ReadLine(), out int year) ||
                 month < 1 ||
                 month > DateTime.Now.Year)
                {
                    goto setBirthdate;
                }

                DateTime birthDate = new DateTime(year, month, day);

                if(birthDate > DateTime.Now)
                {
                    Console.WriteLine("Birth date can't be in the future");
                    goto setBirthdate;
                }
            }
        }

        private static List<Flight> GetFlights()
        {
            csvFileReader = new CsvFileReader(@"C:\Users\JACKY\Desktop\flights.txt");

            var flightCsv = csvFileReader.ReadCsv();

            List<Flight> flights = new List<Flight>();

            foreach (var flight in flightCsv)
            {
                var columns = flight.Split(',');

                flights.Add(new Flight()
                {
                    AirlineCode = columns[0],
                    FlightNumber = int.Parse(columns[1]),
                    ArrivalStation = columns[2],
                    DepartureStation = columns[3],
                    STA = TimeSpan.Parse(columns[4]),
                    STD = TimeSpan.Parse(columns[5])
                });
            }

            return flights;
        }

        private static List<Flight> SearchByCodeAndFlightNumber(string code, int flightNumber)
        {
            var flights = GetFlights();
            return flights.Where(f => f.AirlineCode == code && f.FlightNumber == flightNumber).ToList();
        }

    }
}
