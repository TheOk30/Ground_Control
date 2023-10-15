using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;

namespace Assets
{
    class Airline
    {
        private readonly int airlineID;
        private readonly string airlinename;
        private readonly string airlineCode;
        private List<int> flightNumbers;
        private Plane[] planes;
        private Airport[] airportsFlyingTo;
        public static int NumberOfAirlines;

        public Airline(int airlineID, string airlinename, string airlineCode, Plane[] planes, Airport[] airportsFlyingTo, int numberofAirlines)
        {
            this.airlineID = airlineID;
            this.airlinename = airlinename;
            this.airlineCode = airlineCode;
            this.planes = planes;
            this.airportsFlyingTo = airportsFlyingTo;
            this.flightNumbers = new List<int>();
            NumberOfAirlines = numberofAirlines;
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

        public Plane[] GetPlanes()
        {
            return this.planes;
        }
        
        public static int GetNumberofAirlines()
        {
            return NumberOfAirlines;
        }
    }
}