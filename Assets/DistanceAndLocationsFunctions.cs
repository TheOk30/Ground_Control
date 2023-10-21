using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    abstract class DistanceAndLocationsFunctions
    {
        public const int EarthRadius = 6371;

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

        public static double BearingAngle(double lat1, double lon1, double lat2, double lon2)
        {
            double x = Math.Cos(DegreesToRadians(lat1)) * Math.Sin(DegreesToRadians(lat2)) - Math.Sin(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) * Math.Cos(DegreesToRadians(lon2 - lon1));
            double y = Math.Sin(DegreesToRadians(lon2 - lon1)) * Math.Cos(DegreesToRadians(lat2));

            return (Math.Atan2(y, x) + Math.PI * 2) % (Math.PI * 2);
        }

        public static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0d;
        }

        public static Location GetCoorWithBearingAndDistance(double lat1, double lon1, double bearingAngle, int distanceTraveled)
        {
            //bearing angle 
            double rad = bearingAngle;

            //angDist = distance/radius
            double angDist = distanceTraveled / EarthRadius;

            double latitude = DegreesToRadians(lat1);
            double longitude = DegreesToRadians(lon1);


            double lat = Math.Asin(Math.Sin(latitude) * Math.Cos(angDist) + Math.Cos(latitude) * Math.Sin(angDist) * Math.Cos(rad));

            double forAtana = Math.Sin(rad) * Math.Sin(angDist) * Math.Cos(latitude);
            double forAtanb = Math.Cos(angDist) - Math.Sin(latitude) * Math.Sin(lat);

            double lon = longitude + Math.Atan2(forAtana, forAtanb);
            double finalLat = lat * 180 / Math.PI;
            double finalLon = lon * 180 / Math.PI;

            Location temp = new Location(finalLat, finalLon);
            return temp;
        }

        public static Location GetCoorWithBearingAndDistance(double lat1, double lon1, double lat2, double lon2, int distanceTraveled)
        {
            //bearing angle 
            double rad = BearingAngle(lat1, lon1, lat2, lon2);

            //angDist = distance/radius
            double angDist = distanceTraveled / EarthRadius;

            double latitude = DegreesToRadians(lat1);
            double longitude = DegreesToRadians(lon1);


            double lat = Math.Asin(Math.Sin(latitude) * Math.Cos(angDist) + Math.Cos(latitude) * Math.Sin(angDist) * Math.Cos(rad));

            double forAtana = Math.Sin(rad) * Math.Sin(angDist) * Math.Cos(latitude);
            double forAtanb = Math.Cos(angDist) - Math.Sin(latitude) * Math.Sin(lat);

            double lon = longitude + Math.Atan2(forAtana, forAtanb);
            double finalLat = lat * 180 / Math.PI;
            double finalLon = lon * 180 / Math.PI;

            Location temp = new Location(finalLat, finalLon);
            return temp;
        }
    }
}
