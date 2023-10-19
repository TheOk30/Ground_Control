using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets
{
    public class Airport : Location
    {
        private readonly int airportID;
        private readonly string airportName;
        private readonly string country;
        private readonly string city;

        public Airport(int airportID, string airportName, string country, string city, double latitude, double longitude) :base(latitude, longitude)
        {
            this.airportID = airportID;
            this.airportName = airportName;
            this.country = country;
            this.city = city;
        }

        public Airport(Airport airport) :base(airport.GetLatitude(), airport.GetLongitude())
        {
            this.airportID = airport.GetAirportID();
            this.airportName = airport.GetAirportName();
            this.city = airport.GetCountry();
            this.city = airport.GetCity();
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

        public int DistanceFromCurrentAirport(Location local)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.GetLatitude(), this.GetLongitude(), local.GetLatitude(), local.GetLongitude());
        }

        public static int DistanceBetweenAirports(Airport airport1, Airport airport2)
        {
            return (int)DistanceAndLocationsFunctions.DistanceBetweenCoordinates(airport1.GetLatitude(), airport1.GetLongitude(), airport2.GetLatitude(), airport2.GetLongitude());
        }
    }
}
