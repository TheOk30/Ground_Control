using Assets.Scripts.SolverController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /*
       SpatialHashingLocationIndex
   
       This class implements a spatial hashing technique for indexing and querying locations.
   
       Spatial hashing divides the space into a grid of cells, each represented by a unique hash code.
       Locations are inserted into the spatial hash based on their coordinates, and each location is
       associated with the hash code of the cell it falls into.
   
       When querying for the closest locations to a given query location, the class retrieves nearby
       locations by searching in adjacent cells around the query location's hash code. It then calculates
       the distance between the query location and each nearby location using the Haversine formula
       and returns the closest one.
    */
    public class SpatialHashingLocationIndex
    {
        //Singleton Instance for solver class
        public static SpatialHashingLocationIndex Instance = null;

        private Dictionary<int, List<Location>> spatialHash;
        private int gridSize;

        /// <summary>
        /// Create the Hash Function
        /// </summary>
        private SpatialHashingLocationIndex()
        {
            CalculateGridSize();
            spatialHash = new Dictionary<int, List<Location>>();
            List<Airport> airports = DataBaseManager.Instance.GetAllAirports();

            foreach (Airport airport in airports)
            {
                AddLocation(airport);
            }
        }

        /// <summary>
        /// Add a location to the Hash table
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Location location)
        {
            int hash = ComputeHash(location.GetLatitude(), location.GetLongitude());
            if (!spatialHash.ContainsKey(hash))
                spatialHash[hash] = new List<Location>();
            spatialHash[hash].Add(location);
        }

        /// <summary>
        /// Find Closest Airport to the current location
        /// </summary>
        /// <param name="queryLocation"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public Location FindClosestLocations(Location queryLocation, int grade)
        {
            int queryHash = ComputeHash(queryLocation.GetLatitude(), queryLocation.GetLongitude());
            List<Location> nearbyLocations = new List<Location>();

            for (int i = queryHash - 1; i <= queryHash + 1; i++)
            {
                if (spatialHash.ContainsKey(i))
                    nearbyLocations.AddRange(spatialHash[i]);
            }

            Location closestLocation = null;
            double minDistance = double.MaxValue;

            foreach (Location location in nearbyLocations)
            {
                if(location is Airport && ((Airport)(location)).GetRunwayGrade() >= grade)
                {
                    double distance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(location, queryLocation);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestLocation = location;
                    }
                }
            }

            return closestLocation != null ? closestLocation : null;
        }

        /// <summary>
        /// Create the Hash for each coordinate
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private int ComputeHash(double latitude, double longitude)
        {
            int x = (int)(latitude / gridSize);
            int y = (int)(longitude / gridSize);

            // Use Tuple.Create to create a tuple with latitude and longitude
            var tuple = Tuple.Create(x, y);

            // Get the hash code of the tuple
            return tuple.GetHashCode();
        }

        /// <summary>
        /// Calculate the grid size for the Hash table so we could find how many neighbors we can find
        /// </summary>
        private void CalculateGridSize()
        {
            double width = DataBaseManager.Instance.GetMaxFromAirportTable("Longitude") - DataBaseManager.Instance.GetMinFromAirportTable("Longitude"); 
            double height = DataBaseManager.Instance.GetMaxFromAirportTable("Latitude") - DataBaseManager.Instance.GetMinFromAirportTable("Latitude");
            int gridDivision = 100;

            // Calculate grid size
            double gridSize = Math.Max(width / gridDivision, height / gridDivision);
            this.gridSize = (int)Math.Ceiling(gridSize);
        }

        public static SpatialHashingLocationIndex InitializeSolver()
        {
            if (Instance == null)
                Instance = new SpatialHashingLocationIndex();
            return Instance;
        }
    }
}
