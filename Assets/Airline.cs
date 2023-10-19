using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEditor;

namespace Assets
{
    class Airline
    {
        private readonly int airlineID;
        private readonly string airlinename;
        private readonly string airlineCode;
        private List<int> flightNumbers;
        private List<Plane> planes;
        private List<int> airportsFlyingTo;
        private int homeAirport;
        public static int[] planesCounter;

        public Airline(int airlineID, string airlinename, string airlineCode, List<Plane> planes, List<int> airportsFlyingTo, int homeAirport)
        {
            this.airlineID = airlineID;
            this.airlinename = airlinename;
            this.airlineCode = airlineCode;
            this.planes = planes;
            this.airportsFlyingTo = airportsFlyingTo;
            this.flightNumbers = new List<int>();
            this.homeAirport = homeAirport;
        }

        public int GetAirlineID()
        {
            return airlineID;
        }

        public string GetAirlineName()
        {
            return this.airlinename;
        }

        public string GetAirlineCode()
        {
            return this.airlineCode;
        }

        public List<int> GetFlightNumbers()
        {
            return this.flightNumbers;
        }

        public void AddFlightNumbers(int nums)
        {
            this.flightNumbers.Add(nums);
        }

        public List<Plane> GetPlanes()
        {
            return this.planes;
        }

        public List<Airport> GetAirportsFlyingTo()
        {
            return this.airportsFlyingTo;
        }
    }
}