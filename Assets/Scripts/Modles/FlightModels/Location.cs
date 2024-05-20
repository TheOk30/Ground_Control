using System;

namespace Assets
{
    /// <summary>
    /// Class that holds the information about the location
    /// the location holds the latitude and longitude of the location
    /// </summary>
    public class Location
    {
        private double latitude;
        private double longitude;

        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        /// Get the latitude of the location
        /// </summary>
        /// <returns></returns>
        public double GetLatitude()
        {
            return this.latitude;
        }

        /// <summary>
        /// Get the longitude of the location
        /// </summary>
        /// <returns></returns>
        public double GetLongitude()
        {
            return this.longitude;
        }
    }
}