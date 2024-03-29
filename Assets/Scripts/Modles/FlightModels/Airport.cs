using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets
{
    public class Airport : Location
    {
        private readonly int airportID;
        private readonly string airportName;
        private readonly string country;
        private readonly string city;
        private readonly string AirportCode;
        private readonly int runwayLength;
        private int runwayGrade;

        public Airport(int airportID, string airportName, string country, string city, double latitude, double longitude, string AirportCode, int runwayLength) :base(latitude, longitude)
        {
            this.airportID = airportID;
            this.airportName = airportName;
            this.country = country;
            this.city = city;
            this.AirportCode = AirportCode;
            this.runwayLength = runwayLength;
            this.runwayGrade = CalculateRunwayGrade();
        }

        public int GetAirportID()
        {
            return airportID;
        }

        public string GetAirportName()
        {
            return this.airportName;
        }

        public string GetCountry() 
        {
            return this.country;
        }

        public string GetCity()
        {
            return this.city;
        }

        public string GetAirportCode()
        {
            return this.AirportCode;
        }

        public int GetrunwayLength()
        {
            return this.runwayLength;
        }

        public int GetRunwayGrade()
        {
            return this.runwayGrade;
        }

        /// <summary>
        /// Calculates the grade of the runway based on the runway length.
        /// Will be used to know if a plane could land on that runway when 
        /// deviating flight
        /// </summary>
        /// <returns>The calculated grade.</returns>
        private int CalculateRunwayGrade()
        {
            if (this.runwayLength >= 4000)
                return 10;
            
            else if (this.runwayLength >= 3500)
                return 9;

            else if (this.runwayLength >= 3200)
                return 8;

            else if (this.runwayLength >= 3000)
                return 7;

            else if (this.runwayLength >= 2800)
                return 6;

            else if (this.runwayLength >= 2500)
                return 5;

            else if (this.runwayLength >= 2200)
                return 4;

            else if (this.runwayLength >= 2000)
                return 3;

            else if (this.runwayLength >= 1800)
                return 2;

            else if (this.runwayLength > 0)
                return 1;

            else
                return 0;
        }

        public int DistanceFromCurrentAirport(Location local)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.GetLatitude(), this.GetLongitude(), local.GetLatitude(), local.GetLongitude());
        }

        public static int DistanceBetweenAirports(Airport airport1, Airport airport2)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(airport1.GetLatitude(), airport1.GetLongitude(), airport2.GetLatitude(), airport2.GetLongitude());
        }

        public static void LookForAlternativeAirport(int radius)
        {
            Debug.Log("Need To Look For Alternative Radius");
        }
    }
}