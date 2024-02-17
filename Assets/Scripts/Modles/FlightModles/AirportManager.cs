using Assets.Scripts.Controler;
using Assets.Scripts.Modles.IssuesControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Class that holds the current airport that the simulation is working on
    /// </summary>
    class AirportManager
    {
        private static AirportManager Instance = null;

        private Airport airport;
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
            //// maybe save the past flight schedule in some way
            
            DateTime startScheduleTime = DateTime.UtcNow.Date.AddMinutes(flightStartTime + this.flightIntervals);
            this.flightSchedule = FlightSchedule.CreateFlightSchedule(flightStartTime, startScheduleTime, this.flightIntervals, this.airport);
        }

        private void AddProblemsToFligths()
        {
            System.Random rnd = new System.Random();
            foreach (Flight flight in this.flightSchedule.GetFlights())
            {
                //flight chosen to have a problem 
                if (rnd.Next(1, 101) % SimulationController.percentageOfProblem == 0)
                {
                    flight.AddProblemToFlight(new ProblemCreator(flight.GetFlightDurationMinutes(), flight.GetTakeOffTime()));
                } 
            }
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

