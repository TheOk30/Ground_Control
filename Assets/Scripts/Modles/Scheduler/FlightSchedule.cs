using Assets.Scripts.Controller;
using Assets.Scripts.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets
{
    class FlightSchedule
    {
        private MinHeap<Flight> flights;
        private readonly DateTime date;
        private double flightIntervals;
        private int numberOfDailyFlights;
        private Airport MainAirport;
        public static FlightSchedule Instance = null;
        private int numRunways;
        public static int weatherIndex = 0;

        private FlightSchedule(int flightStartTime, DateTime date, int flightIntervals, Airport MainAirport, int numRunways)
        {
            this.flightIntervals = flightIntervals;
            this.numberOfDailyFlights = (int)Math.Ceiling((24 - flightStartTime) * (60.0 / this.flightIntervals));
            this.flights = new MinHeap<Flight>();
            this.date = date;
            Debug.Log("Creating Flight Schedule for " + date.ToString());
            this.MainAirport = MainAirport;
            this.numRunways = numRunways;

            weatherIndex = (SimulationController.rnd.Next(0, 100) % 7) == 0 ? SimulationController.rnd.Next(1, 4) : 0;
            CreateFlights();
        }

        /// <summary>
        /// Create flights for the flight schedule
        /// </summary>
        private void CreateFlights()
        {
            DateTime flightscheduleDate = this.date;
            
            for (int i = 0; i < this.numberOfDailyFlights; i++)
            {
                this.flights.Insert(AddFlight(flightscheduleDate.AddMinutes(i* flightIntervals)));
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
            int mainIsTakeOff = SimulationController.rnd.Next(2);
            int runwayChosen = SimulationController.rnd.Next(1, this.numRunways+1);

            //Get a list of all the airlines that are flying to the main airport
            List<int> AirlineFlyingToCurrentAirport = DataBaseManager.Instance.GetAirlinesFlyingToAirport(this.MainAirport.GetAirportID());

            Plane plane = null;
            Airline airline = null;
            Airport otherAirport = null;

            while (plane == null)
            {
                //generate random airline
                int rndAirlineIndex = SimulationController.rnd.Next(1, AirlineFlyingToCurrentAirport.Count);
                airline = DataBaseManager.Instance.GetAllAirlineInfo(rndAirlineIndex);
                int maxDistanceForAirline = DataBaseManager.Instance.GetMaxFlightDistanceForAirline(rndAirlineIndex);

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

                if (flightDistance < maxDistanceForAirline)
                {
                    plane = DataBaseManager.Instance.GetRandomPlane(airline.GetAirlineID(), flightDistance);
                    AirlineFlyingToCurrentAirport.Remove(rndAirlineIndex);
                }
            } 

            airline.BindPlaneToAirline(plane);

            if (mainIsTakeOff == 1)
            {
                return new Flight(airline, plane, this.MainAirport, otherAirport, time, runwayChosen);
            }

            return new Flight(airline, plane, otherAirport, this.MainAirport, time, runwayChosen);
        }

        /// <summary>
        /// Make sure value adhere to the rules of the airport
        /// </summary>
        //public void AreValuesBetweenNeighborsUnderThreshold()
        //{
        //    List<Flight> values = this.flights.GetSortedWithoutModifyingHeap();
        //    for (int i = 1; i < values.Count; i++)
        //    {
        //        TimeSpan difference = values[i].GetTimeToCompare() - values[i - 1].GetTimeToCompare();

        //        if ((int)difference.TotalSeconds < (SimulationController.TimeBetweenFlightsOnSchedule + SimulationController.Weather[weatherIndex]) * 60)
        //        {
        //            // Calculate the difference between the threshold and the difference between two flights
        //            int differenceThreshold = SimulationController.TimeBetweenFlightsOnSchedule - (int)difference.TotalSeconds;

        //            // Adjust the time of current flight if it's landing at main
        //            if (values[i].GetIsFlightLandingAtMain())
        //            {
        //                values[i].ChangeLandingTime(differenceThreshold);
        //            }

        //            // Adjust the time of current flight if it's taking off at main
        //            else
        //            {
        //                values[i].ChangeTakeoffTime(differenceThreshold);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Get the Date of the flight schedule
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate()       
        { 
            return date; 
        } 

        /// <summary>
        /// Get the number of daily flights
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfDailyFlights()
        {
            return this.numberOfDailyFlights;
        }

        /// <summary>
        /// Get the flights in heap format
        /// </summary>
        /// <returns></returns>
        public MinHeap<Flight> GetFlights()
        {
            return this.flights;
        }

        /// <summary>
        /// Create the flight schedule object singleton
        /// activated by the main airport class
        /// </summary>
        /// <param name="flightStartTime"></param>
        /// <param name="date"></param>
        /// <param name="flightIntervals"></param>
        /// <param name="MainAirport"></param>
        /// <param name="numRunways"></param>
        /// <returns></returns>
        public static FlightSchedule CreateFlightSchedule(int flightStartTime, DateTime date, int flightIntervals, Airport MainAirport, int numRunways)
        {
            if(Instance == null) 
                Instance = new FlightSchedule(flightStartTime, date, flightIntervals, MainAirport, numRunways);
            return Instance;
        }

        /// <summary>
        /// to string function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "";

            if (this.flights.GetSize() != 0)
            {
                foreach (Flight flight in this.flights.GetHeap())
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
