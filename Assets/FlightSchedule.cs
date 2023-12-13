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
        private double flightIntervals;
        private int numberOfDailyFlights;
        private Airport MainAirport;
        public static int numberOfFlightsOnSchedule = 0;
        private static FlightSchedule Instance = null;

        private FlightSchedule(int flightStartTime, DateTime date, int flightIntervals, Airport MainAirport)
        {
            this.flightIntervals = flightIntervals;
            this.numberOfDailyFlights = (int)Math.Ceiling((24 - flightStartTime) * (60.0 / this.flightIntervals));
            this.flights = new Flight[this.numberOfDailyFlights-1];
            this.date = date;
            this.MainAirport = MainAirport;

            CreateFlights();
        }

        private void CreateFlights()
        {
            DateTime flightscheduleDate = this.date;
            
            for (int i = 0; i < this.flights.Length; i++)
            {
                this.flights[i] = AddFlight(flightscheduleDate.AddMinutes(i* flightIntervals));
            }
        }
        
        /// <summary>
        /// Add a flight for the schedule working per day
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private Flight AddFlight(DateTime time) 
        {
            //calculates if the Main airport is takeoff or not
            Random rnd = new Random();
            int mainIsTakeOff = rnd.Next(2);

            //Get a list of all the airlines that are flying to the main airport
            List<int> AirlineFlyingToCurrentAirport = DataBaseManager.Instance.GetAirlinesFlyingToAirport(this.MainAirport.GetAirportID());

            Plane plane = null;
            Airline airline = null;
            Airport otherAirport = null;

            while (plane == null)
            {
                //generate random airline
                int rndAirlineIndex = rnd.Next(0, AirlineFlyingToCurrentAirport.Count);
                airline = DataBaseManager.Instance.GetAllAirlineInfo(rndAirlineIndex);

                int otherAirportID = 0;
                if (airline.GetHomeAirport() != 0 && airline.GetHomeAirport() != this.MainAirport.GetAirportID())
                {
                    otherAirportID = airline.GetHomeAirport();
                }

                else
                {
                    otherAirportID = DataBaseManager.Instance.SelectRandomAirportIdFromTable(rndAirlineIndex, this.MainAirport.GetAirportID());
                }


                otherAirport = DataBaseManager.Instance.GetAllAirportInfo(otherAirportID);

                int flightDistance = Airport.DistanceBetweenAirports(this.MainAirport, otherAirport);

                plane = DataBaseManager.Instance.GetRandomPlane(airline.GetAirlineID(), flightDistance);
                AirlineFlyingToCurrentAirport.Remove(rndAirlineIndex);
            } 

            airline.BindPlaneToAirline(plane);

            if (mainIsTakeOff == 1)
            {
                return new Flight(airline, plane, this.MainAirport, otherAirport, time);
            }

            return new Flight(airline, plane, otherAirport, this.MainAirport, time);
        }

        public DateTime GetDate()       
        { 
            return date; 
        } 

        public static int GetNumberOfFlightsOnSchedule()
        {
            return numberOfFlightsOnSchedule;
        }

        public Flight[] GetFlights()
        {
            return this.flights;
        }
        public static FlightSchedule CreateFlightSchedule(int flightStartTime, DateTime date, int flightIntervals, Airport MainAirport)
        {
            if(Instance == null || DateTime.Compare(Instance.GetDate(), date) > 0) 
                Instance = new FlightSchedule(flightStartTime, date, flightIntervals, MainAirport);
            return Instance;
        }

        public override string ToString()
        {
            string str = "";

            if (this.flights.Count() != 0)
            {
                foreach (Flight flight in this.flights)
                {
                    if (flight != null)
                    {
                        str += flight.ToString("HH:mm") + "\n";
                    }
                }
            }

            return str;
        }
    }
}
