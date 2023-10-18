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
        public static int numberOfFlightsOnSchedule = 0;
        private static FlightSchedule Instance = null;
        private FlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals)
        {
            this.numberOfDailyFlights = numberOfDailyFlights;
            this.flights = new Flight[this.numberOfDailyFlights];
            this.date = date;
            this.flightIntervals = flightIntervals;

            CreateFlights();
        }

        private void CreateFlights()
        {
            DateTime flightscheduleDate = this.date.Date.Add(new TimeSpan(0,0,0));
            
            for (int i = 0; i < this.flights.Length && this.numberOfDailyFlights > numberOfFlightsOnSchedule; i++)
            {
                this.flights[i] = AddFlight(flightscheduleDate);
            }
        }

        public Flight AddFlight(DateTime time) 
        {
            Random rnd = new Random();
            
        }

        public DateTime GetDate()       
        { 
            return date; 
        } 

        public static int GetNumberOfFlightsOnSchedule()
        {
            return numberOfFlightsOnSchedule;
        }

        public static FlightSchedule CreateFlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals)
        {
            if(Instance == null || DateTime.Compare(Instance.GetDate(), date) > 0) 
                Instance = new FlightSchedule(numberOfDailyFlights, date, flightIntervals);
            return Instance;
        }
    }
}
