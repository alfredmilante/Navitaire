﻿using NavLibrary;
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
        private static Random random = new Random();

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

            List<Passenger> passengers = new List<Passenger>();

            for (int i = 1; i <= passengerCount; i++)
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

                Console.WriteLine("Day: (01-31)");
                if (!int.TryParse(Console.ReadLine(), out int day) ||
                 month < 1 ||
                 month > 31)
                {
                    goto setBirthdate;
                }


                Console.WriteLine("Year:");
                if (!int.TryParse(Console.ReadLine(), out int year) ||
                 month < 1 ||
                 month > DateTime.Now.Year)
                {
                    goto setBirthdate;
                }

                DateTime birthDate = new DateTime(year, month, day);

                if (birthDate > DateTime.Now)
                {
                    Console.WriteLine("Birth date can't be in the future");
                    goto setBirthdate;
                }

                passengers.Add(new Passenger()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate
                });
            }

            Console.WriteLine("Reservation Details: ");

            foreach (var flight in flights)
            {
                Console.WriteLine(string.Format(
             "Airline Code: {0}, " +
             "Flight Number: {1}, " +
             "Arrival Station: {2}, " +
             "Departure Station: {3}, " +
             "STA: {4}, " +
             "STD: {5} \n", flight.AirlineCode, flight.FlightNumber, flight.ArrivalStation, flight.DepartureStation, flight.STA, flight.STD));
            }


            for (int i = 1; i <= passengerCount; i++)
            {
                Console.Write($"Passenger {i} - ");
                Console.WriteLine(string.Format(
                 "First Name: {0}, " +
                 "Last Name: {1}, " +
                 "Birth Date: {2}, " +
                 "Age: {3} \n", passengers[i - 1].FirstName, passengers[i - 1].LastName, passengers[i - 1].BirthDate, passengers[i - 1].Age));
            }

        checkConfirmation:
            Console.WriteLine("Is your reservation confirmed? (YES/NO)");
            var response = Console.ReadLine();

            if (response == "YES")
            {
                var PNR = AddReservation(new Reservation()
                {
                    AirlineCode = airlineCode,
                    FlightNumber = flightNumber,
                    FlightDate = DateTime.Now.Date,
                    NoOfPassengers = passengerCount
                }, passengers);
            }
            else if (response == "NO")
            {
                goto reservation;
            }
            else
            {
                goto checkConfirmation;
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

        private static List<Reservation> GetReservations()
        {
            csvFileReader = new CsvFileReader(@"C:\Users\JACKY\Desktop\reservations.csv");

            var reservationCsv = csvFileReader.ReadCsv();

            List<Reservation> reservations = new List<Reservation>();

            foreach (var reservation in reservationCsv)
            {
                var columns = reservation.Split(',');

                reservations.Add(new Reservation()
                {
                    PNR = columns[0],
                    AirlineCode = columns[1],
                    FlightNumber = int.Parse(columns[2]),
                    FlightDate = DateTime.Parse(columns[3]),
                    NoOfPassengers = int.Parse(columns[4]),

                });
            }

            return reservations;
        }

        public static string GeneratePNR()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var firstLetter = new string(Enumerable.Repeat(letters, 1)
              .Select(s => s[random.Next(letters.Length-1)]).ToArray());
            var pnr = new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(chars.Length-1)]).ToArray());

            return firstLetter + pnr;
        }

        private static string AddReservation(Reservation reservation, List<Passenger> passengers)
        {
            var existingReservations = GetReservations();

        generatePNR:
            var pnr = GeneratePNR();

            if (existingReservations.FirstOrDefault(r => r.PNR == pnr) != null)
            {
                goto generatePNR;
            }
            try
            {
                var row = new List<string>()
                {
                  pnr,
                  reservation.AirlineCode,
                  reservation.FlightNumber.ToString(),
                  $"ss",
                  reservation.NoOfPassengers.ToString()
                };

                csvFileWriter = new CsvFileWriter(@"C:\Users\JACKY\Desktop\reservations.csv");
                csvFileWriter.WriteRow(row);

                foreach (var passenger in passengers)
                {
                    AddPassenger(passenger, pnr);
                }

                return pnr;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static void AddPassenger(Passenger passenger, string pnr)
        {
            csvFileWriter = new CsvFileWriter(@"C:\Users\JACKY\Desktop\passengers.txt");

            try
            {
                var row = new List<string>()
                {
                  pnr,
                  passenger.FirstName,
                  passenger.LastName,
                  passenger.BirthDate.ToString(),
                  passenger.Age.ToString()
                };

                csvFileWriter.WriteRow(row);
            }
            catch (Exception ex)
            {
            }
        }

        private static List<Flight> SearchByCodeAndFlightNumber(string code, int flightNumber)
        {
            var flights = GetFlights();
            return flights.Where(f => f.AirlineCode == code && f.FlightNumber == flightNumber).ToList();
        }



    }
}
