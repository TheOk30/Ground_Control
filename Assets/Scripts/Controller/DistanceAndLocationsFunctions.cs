using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    abstract class DistanceAndLocationsFunctions
    {
        public const double EarthRadius = 6371.0;

        /// <summary>
        /// Get the angular distance between two coordinates of the Earth
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        public static int DistanceBetweenCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae: Haversine formula
            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);

            // Radius of earth in kilometers
            double rad = EarthRadius;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return (int)(rad * c);
        }

        /// <summary>
        /// Get the angular distance between two coordinates of the Earth
        /// </summary>
        /// <param name="local1"></param>
        /// <param name="local2"></param>
        /// <returns></returns>
        public static int DistanceBetweenCoordinates(Location local1, Location local2)
        {
            double lat1 = local1.GetLatitude();
            double lon1 = local1.GetLongitude();
            double lat2 = local2.GetLatitude();
            double lon2 = local2.GetLongitude();

            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae: Haversine formula
            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);

            // Radius of earth in kilometers
            double rad = EarthRadius;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return (int)(rad * c);
        }

        /// <summary>
        /// Find the bearing anlge between two coordinates
        /// used to find a coordinate with just distanced travled and 
        /// starting coordinate
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        public static double BearingAngle(double lat1, double lon1, double lat2, double lon2)
        {
            double y = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1);
            double bearing = Math.Atan2(y, x);
            return (bearing + 2 * Math.PI) % (2 * Math.PI);
        }

        /// <summary>
        /// Convert angle From degrees to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        /// <summary>
        /// Convert from radians to degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Find the Current Coordinate Of the plane with the starting location
        /// Distance from the Starting Location and bearing angle
        /// </summary>
        /// <param name="departureLatitude"></param>
        /// <param name="departureLongitude"></param>
        /// <param name="arrivalLatitude"></param>
        /// <param name="arrivalLongitude"></param>
        /// <param name="distanceFromDeparture"></param>
        /// <returns></returns>
        public static Location GetCoorWithBearingAndDistance(double departureLatitude, double departureLongitude,double arrivalLatitude, double arrivalLongitude,
            double distanceFromDeparture)
        {
            // Convert degrees to radians
            departureLatitude = DegreesToRadians(departureLatitude);
            departureLongitude = DegreesToRadians(departureLongitude);
            arrivalLatitude = DegreesToRadians(arrivalLatitude);
            arrivalLongitude = DegreesToRadians(arrivalLongitude);

            // Calculate the angular distance
            double angularDistance = distanceFromDeparture / EarthRadius;

            // Calculate the bearing angle between the two points
            double brng = BearingAngle(departureLatitude, departureLongitude, arrivalLatitude, arrivalLongitude);

            // Convert distance traveled to angular distance (in radians)
            double d = distanceFromDeparture / EarthRadius; // Earth radius in kilometers

            // Calculate the new latitude using the Vincenty formula
            double lat2 = Math.Asin(Math.Sin(departureLatitude) * Math.Cos(d) +
                          Math.Cos(departureLatitude) * Math.Sin(d) * Math.Cos(brng));

            // Calculate the new longitude using the Vincenty formula
            double lon2 = departureLongitude + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(departureLatitude),
                                               Math.Cos(d) - Math.Sin(departureLatitude) * Math.Sin(lat2));

            // Convert the new latitude and longitude from radians to degrees
            lat2 = RadiansToDegrees(lat2);
            lon2 = RadiansToDegrees(lon2);

            // Create a new Location object with the calculated coordinates
            Location temp = new Location(lat2, lon2);

            // Log the flight location for debugging purposes
            UnityEngine.Debug.Log("flight location in function: " + temp);

            // Return the calculated location
            return temp;
        }

        public static Location OptimumSearchRange(Location queryLocation)
        {
            // Calculate the equivalent latitude and longitude degrees for the search radius (e.g., 10 kilometers)
            const double EarthRadiusM = 6371000; // Earth radius in meters
            const double SearchRadius = 10000; // Search radius in meters (10 kilometers)

            // Calculate the distance corresponding to one degree of latitude at the equator
            double latitudeDegreeDistance = 2 * Math.PI * EarthRadiusM / 360;

            // Calculate the distance corresponding to one degree of longitude at the equator
            double longitudeDegreeDistance = latitudeDegreeDistance * Math.Cos(Math.PI / 180 * queryLocation.GetLatitude());

            // Calculate the equivalent number of degrees for the search radius
            double latitudeDegrees = SearchRadius / latitudeDegreeDistance;
            double longitudeDegrees = SearchRadius / longitudeDegreeDistance;

            // Expand the search area by adjusting the search range based on latitude and longitude degrees
            int searchRangeLatitude = (int)Math.Ceiling(latitudeDegrees);
            int searchRangeLongitude = (int)Math.Ceiling(longitudeDegrees);

            return new Location(searchRangeLatitude, searchRangeLongitude);
        }
    }
}