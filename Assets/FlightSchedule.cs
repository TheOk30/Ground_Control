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
        private Airport MainAirportId;
        public static int numberOfFlightsOnSchedule = 0;
        private static FlightSchedule Instance = null;
        private FlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals, Airport airport)
        {
            this.numberOfDailyFlights = numberOfDailyFlights;
            this.flights = new Flight[this.numberOfDailyFlights];
            this.date = date;
            this.flightIntervals = flightIntervals;
            this.MainAirportId = airport;

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
        
        private Flight AddFirstFlight()
        {
            Random rnd = new Random();
            int mainIsTakeOff = 1;

            List<int> AirlinedFlyingToCurrentAirport = DataBaseManager.Instance.GetAirlinesFlyingToAirport(this.MainAirportId.GetAirportID());

            int rndAirlineIndex = rnd.Next(0, AirlinedFlyingToCurrentAirport.Count);
            Airline airline = DataBaseManager.Instance.GetAllAirlineInfo(this.MainAirportId.GetAirportID());

            int otherAirportID;
            if (airline.GetHomeAirport() != 0 && airline.GetHomeAirport() != this.MainAirportId.GetAirportID())
            {
                otherAirportID = airline.GetHomeAirport();
            }

            else
            {
                otherAirportID = DataBaseManager.Instance.SelectRandomAirportIdFromTable(rndAirlineIndex, this.MainAirportId.GetAirportID());
            }


            Airport otherAirport = DataBaseManager.Instance.GetAllAirportInfo(otherAirportID);

            int flightDistance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.MainAirportId, otherAirport);


        }
        private Flight AddFlight(DateTime time) 
        {
            Random rnd = new Random();
            int  mainIsTakeOff = rnd.Next(2);

            List<int> AirlinedFlyingToCurrentAirport = DataBaseManager.Instance.GetAirlinesFlyingToAirport(this.MainAirportId.GetAirportID());

            int rndAirlineIndex = rnd.Next(0, AirlinedFlyingToCurrentAirport.Count);
            Airline airline = DataBaseManager.Instance.GetAllAirlineInfo(this.MainAirportId.GetAirportID());

            int otherAirportID;
            if (airline.GetHomeAirport() != 0 && airline.GetHomeAirport() != this.MainAirportId.GetAirportID())
            {
                 otherAirportID = airline.GetHomeAirport();
            }

            else
            {
                otherAirportID = DataBaseManager.Instance.SelectRandomAirportIdFromTable(rndAirlineIndex, this.MainAirportId.GetAirportID());
            }
           
           
            Airport otherAirport = DataBaseManager.Instance.GetAllAirportInfo(otherAirportID);

            int flightDistance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.MainAirportId, otherAirport);

            return new Flight();
        }

        private Flight AddFlight(DateTime time, )
        {

        }

        public DateTime GetDate()       
        { 
            return date; 
        } 

        public static int GetNumberOfFlightsOnSchedule()
        {
            return numberOfFlightsOnSchedule;
        }

        public static FlightSchedule CreateFlightSchedule(int numberOfDailyFlights, DateTime date, int flightIntervals, Airport MainAirportId)
        {
            if(Instance == null || DateTime.Compare(Instance.GetDate(), date) > 0) 
                Instance = new FlightSchedule(numberOfDailyFlights, date, flightIntervals, MainAirportId);
            return Instance;
        }
    }
}
