using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Class that holds the information about the airport
    /// </summary>
    public class Airport : Location
    {
        private readonly int airportID;
        private readonly string airportName;
        private readonly string country;
        private readonly string city;
        private readonly string AirportCode;
        private readonly int runwayLength;
        private int runwayGrade;

        public Airport(int airportID, string airportName, string country, string city, double latitude, 
                        double longitude, string AirportCode, int runwayLength) :base(latitude, longitude)
        {
            this.airportID = airportID;
            this.airportName = airportName;
            this.country = country;
            this.city = city;
            this.AirportCode = AirportCode;
            this.runwayLength = runwayLength;
            this.runwayGrade = CalculateRunwayGrade();
        }

        /// <summary>
        /// Function that returns the airport ID
        /// </summary>
        /// <returns></returns>
        public int GetAirportID()
        {
            return airportID;
        }

        /// <summary>
        /// function that returns the airport name
        /// </summary>
        /// <returns></returns>
        public string GetAirportName()
        {
            return this.airportName;
        }

        /// <summary>
        /// fucntion that returns the country of the airport
        /// </summary>
        /// <returns></returns>
        public string GetCountry() 
        {
            return this.country;
        }

        /// <summary>
        /// function that returns the city of the airport
        /// </summary>
        /// <returns></returns>
        public string GetCity()
        {
            return this.city;
        }

        /// <summary>
        /// function that returns the airport code
        /// </summary>
        /// <returns></returns>
        public string GetAirportCode()
        {
            return this.AirportCode;
        }

        /// <summary>
        /// function that returns the runway length
        /// </summary>
        /// <returns></returns>
        public int GetRunwayLength()
        {
            return this.runwayLength;
        }

        /// <summary>
        /// function that returns the runway grade
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// function that returns the distance from the current airport
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public int DistanceFromCurrentAirport(Location local)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.GetLatitude(), this.GetLongitude(), local.GetLatitude(), local.GetLongitude());
        }

        /// <summary>
        /// function that returns the distance between two airports
        /// </summary>
        /// 
        /// <param name="airport1"></param>
        /// <param name="airport2"></param>
        /// <returns></returns>
        public static int DistanceBetweenAirports(Airport airport1, Airport airport2)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(airport1.GetLatitude(), airport1.GetLongitude(), airport2.GetLatitude(), airport2.GetLongitude());
        }
    }
}