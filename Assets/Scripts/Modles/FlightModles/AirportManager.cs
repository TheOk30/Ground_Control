using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets
{
    class AirportManager
    {
        private static AirportManager Instance = null;

        private Airport airport;
        private FlightSchedule flightSchedule;
        private int flightIntervals;

        private AirportManager(Airport airport, int flightIntervals)
        {
            this.airport = airport;
            this.flightSchedule = null;
            this.flightIntervals = flightIntervals;
        }

        public FlightSchedule GetFlightSchedule()
        {
            return this.flightSchedule;
        }

        public void CreateFlightScheduleForAirport(int flightStartTime)
        {
            //// maybe save the past flight schedule in some way
            
            DateTime startScheduleTime = DateTime.UtcNow.Date.AddMinutes(flightStartTime + this.flightIntervals);
            Console.WriteLine(startScheduleTime);
            this.flightSchedule = FlightSchedule.CreateFlightSchedule(flightStartTime, startScheduleTime, this.flightIntervals, this.airport);
        }


        public static AirportManager InitializeAirportManager(Airport airport, int flightIntervals)
        {
            if (Instance == null)
                Instance = new AirportManager( airport, flightIntervals);
            return Instance;
        }

    }
}

