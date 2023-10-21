using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class AirportManager :Airport
    {
        private FlightSchedule flightSchedule;
        private static AirportManager Instance = null;

        private AirportManager(Airport airport, int numberOfDailyFlights, int flightIntervals) : base(airport)
        {
            this.flightSchedule = FlightSchedule.CreateFlightSchedule(numberOfDailyFlights, DateTime.Today, flightIntervals, Airport airport);
        }

        public static AirportManager InitializeAirportManager(Airport airport, int numberOfDailyFlights, int flightIntervals)
        {
            if (Instance == null)
                Instance = new AirportManager(airport, numberOfDailyFlights, flightIntervals);
            return Instance;
        }

    }
}
