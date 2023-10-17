using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class FlightSchedule
    {
        private Flight[] flights;
        private DateTime date;
        private int numberOfDailyFlights;
        public static int numberOfFLightsOnSchedule = 0;

        public FlightSchedule(int numberOfDailyFlights, DateTime date)
        {
            this.flights = new Flight[numberOfDailyFlights];
            this.numberOfDailyFlights = numberOfDailyFlights;
            this.date = date;
        }

        public bool AddFLight(Flight flight) 
        {
            if (numberOfDailyFlights > numberOfFLightsOnSchedule) ;
            {
                this.flights[numberOfFLightsOnSchedule] = flight;
                numberOfFLightsOnSchedule++;
                return true;
            }
            return false;
        }

        public static int GetNumberOfFLightsOnSchedule()
        {
            return numberOfFLightsOnSchedule;
        }
    }
}
