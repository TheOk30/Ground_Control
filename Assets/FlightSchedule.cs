using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets
{
    class FlightSchedule
    {
        private Flight[] flights;
        private readonly DateTime date;
        private int flightIntervals;
        private int numberOfDailyFlights;
        private int MainAirportId;
        public static int numberOfFlightsOnSchedule = 0;
        private static FlightSchedule Instance = null;
        private FlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals, int MainAirportId)
        {
            this.numberOfDailyFlights = numberOfDailyFlights;
            this.flights = new Flight[this.numberOfDailyFlights];
            this.date = date;
            this.flightIntervals = flightIntervals;
            this.MainAirportId = MainAirportId;

            CreateFlights();
        }

        private void CreateFlights()
        {
            DateTime flightscheduleDate = this.date;
            
            for (int i = 0; i < this.flights.Length && this.numberOfDailyFlights > numberOfFlightsOnSchedule; i++)
            {
                this.flights[i] = AddFlight(flightscheduleDate);
            }
        }

        private Flight AddFlight(DateTime time) 
        {
            Random rnd = new Random();

            List<int> AirlinedFlyingToCurrentAirport = DataBaseManager.Instance.GetAirlinesFlyingToAirport(this.MainAirportId);

            int rndAirlineIndex = rnd.Next(0, AirlinedFlyingToCurrentAirport.Count);
            Airline airline = DataBaseManager.Instance
            Airport otherAirport = DataBaseManager.Instance.GetAllAirportInfo(rnd.Next();
            return new Flight();
        }

        public DateTime GetDate()       
        { 
            return date; 
        } 

        public static int GetNumberOfFlightsOnSchedule()
        {
            return numberOfFlightsOnSchedule;
        }

        public static FlightSchedule CreateFlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals ,int MainAirportId)
        {
            if(Instance == null || DateTime.Compare(Instance.GetDate(), date) > 0) 
                Instance = new FlightSchedule(numberOfDailyFlights, date, flightIntervals, int MainAirportId);
            return Instance;
        }
    }
}
