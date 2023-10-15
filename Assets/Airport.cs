using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets
{

    class Airport : Location
    {
        private readonly int airportID;
        private readonly string airportName;
        private readonly string Country;
        private readonly string City;
        public static int numberOfAirpots;

        public Airport(int airportID, string airportName, string Country, string City, double latitude, double longitude, int numberOfAirpotsParm) :base(latitude, longitude)
        {
            this.airportID = airportID;
            this.airportName = airportName;
            this.Country = Country;
            this.City = City;
            numberOfAirpots = numberOfAirpotsParm;
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
            return this.Country;
        }

        public string GetCity()
        {
            return this.City;
        }

        public int DistanceFromCurrentAirport(Location local)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.GetLatitude(), this.GetLongitude(), local.GetLatitude(), local.GetLongitude());
        }

        public static int DistanceBetweenAirports(Airport airport1, Airport airport2)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(airport1.GetLatitude(), airport1.GetLongitude(), airport2.GetLatitude(), airport2.GetLongitude());
        }

        public static int GetNumberOfAirpots()
        {
            return numberOfAirpots;
        }
    }
}
