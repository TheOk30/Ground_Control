using Assets.Scripts.Controller;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Modles.IssuesControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Class that holds the current airport that the simulation is working on
    /// </summary>
    class AirportManager
    {
        public static AirportManager Instance = null;

        private readonly Airport airport;
        private FlightSchedule flightSchedule;
        private List<Flight> removedFligths;
        private List<Flight> landedFligths;
        private int flightIntervals;
        private int numReorders;
        private int numRunways;

        private AirportManager(Airport airport, int flightIntervals, int numRunways)
        {
            this.airport = airport;
            this.flightSchedule = null;
            this.removedFligths = new List<Flight>();
            this.landedFligths = new List<Flight>();
            this.flightIntervals = flightIntervals;
            this.numRunways = numRunways;
        }

        /// <summary>
        /// Create the flight Schedule for each new day
        /// </summary>
        /// <param name="flightStartTime"></param>
        public void CreateFlightScheduleForAirport(int flightStartTime)
        {
            // maybe save the past flight schedule in some way
            DateTime today = DateTime.Today;
            DateTime startScheduleTime = new DateTime(today.Year, today.Month, today.Day, 1, 0, 0);
            this.flightSchedule = FlightSchedule.CreateFlightSchedule(flightStartTime, startScheduleTime, this.flightIntervals, this.airport, this.numRunways);
            //this.flightSchedule.AreValuesBetweenNeighborsUnderThreshold();
            AddProblemsToFlights();
        }

        /// <summary>
        /// Every flight has a problem object in them. However only sum of them are active.
        /// This method allows for encapsulation and lets the problem creator act completely
        /// As a "BlackBox" and no flight can know if it has a problem or not
        /// </summary>
        private void AddProblemsToFlights()
        {
            foreach (Flight flight in this.flightSchedule.GetFlights().GetHeap())
            {
                flight.AddProblemToFlight(new ProblemCreator(flight.GetFlightDurationMinutes(), flight.GetTakeOffTime()));
            }
        }

        /// <summary>
        /// Get the main airport that the simulation is working on
        /// </summary>
        /// <returns></returns>
        public Airport GetMainAirport()
        {
            return this.airport;
        }

        /// <summary>
        /// Get The Flight Schedule
        /// </summary>
        /// <returns></returns>
        public FlightSchedule GetFlightSchedule()
        {
            return this.flightSchedule;
        }

        /// <summary>
        /// Get the flight that have been removed
        /// </summary>
        /// <param name="flight"></param>
        public void AddFlightToRemovedFlights(Flight flight)
        {
            this.removedFligths.Add(flight);
        }

        /// <summary>
        /// Get the flight intervals
        /// </summary>
        /// <param name="flight"></param>
        public void AddFlightToLandedFlights(Flight flight)
        {
            this.landedFligths.Add(flight);
        }

        /// <summary>
        /// Get the removed flights
        /// </summary>
        /// <returns></returns>
        public List<Flight> GetRemovedFlights()
        {
            return this.removedFligths;
        }

        /// <summary>
        /// Get the flights that havw landed
        /// </summary>
        /// <returns></returns>
        public List<Flight> GetLandedFlights()
        {
            return this.landedFligths;
        }

        /// <summary>
        /// Get the number of the reorders that have been done
        /// </summary>
        /// <returns></returns>
        public int GetNumReorders()
        {
            return this.numReorders;
        }

        /// <summary>
        /// Add a new reorder to the list
        /// </summary>
        public void newReorder()
        {
            this.numReorders++;
        }

        /// <summary>
        /// Get the number of runways
        /// </summary>
        /// <returns></returns>
        public int GetNumRunways()
        {
            return this.numRunways;
        }

        /// <summary>
        /// Get the flight intervals
        /// </summary>
        /// <returns></returns>
        public int GetFlightIntervals()
        {
            return this.flightIntervals;
        }   
        
        /// <summary>
        /// Initialize the airport manager singleton
        /// </summary>
        /// <param name="airport"></param>
        /// <param name="flightIntervals"></param>
        /// <param name="numRunways"></param>
        /// <returns></returns>
        public static AirportManager InitializeAirportManager(Airport airport, int flightIntervals, int numRunways)
        {
            if (Instance == null)
                Instance = new AirportManager(airport, flightIntervals, numRunways);
            return Instance;
        }
    }
}