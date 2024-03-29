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
        private int flightIntervals;
        private int numRunways;

        private AirportManager(Airport airport, int flightIntervals, int numRunways)
        {
            this.airport = airport;
            this.flightSchedule = null;
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
            
            DateTime startScheduleTime = DateTime.UtcNow.Date.AddMinutes(flightStartTime + this.flightIntervals);
            this.flightSchedule = FlightSchedule.CreateFlightSchedule(flightStartTime, startScheduleTime, this.flightIntervals, this.airport, this.numRunways);
            this.flightSchedule.AreValuesBetweenNeighborsUnderThreshold();
            AddProblemsToFligths();
        }

        /// <summary>
        /// Every flight has a problem object in them. However only sum of them are active.
        /// This method allows for encapsulation and lets the problem creator act completely
        /// As a "BlackBox" and no flight can know if it has a problem or not
        /// </summary>
        private void AddProblemsToFligths()
        {
            foreach (Flight flight in this.flightSchedule.GetFlights().GetHeap())
            {
                flight.AddProblemToFlight(new ProblemCreator(flight.GetFlightDurationMinutes(), flight.GetTakeOffTime()));
            }
        }

        public Airport GetMainAirport()
        {
            return this.airport;
        }

        public FlightSchedule GetFlightSchedule()
        {
            return this.flightSchedule;
        }

        public static AirportManager InitializeAirportManager(Airport airport, int flightIntervals, int numRunways)
        {
            if (Instance == null)
                Instance = new AirportManager(airport, flightIntervals, numRunways);
            return Instance;
        }
    }
}