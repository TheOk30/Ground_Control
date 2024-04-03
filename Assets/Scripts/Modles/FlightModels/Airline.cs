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
    /// <summary>
    /// Airline Class
    /// </summary>
    public class Airline
    {
        private readonly int airlineID;
        private readonly string airlineName;
        private readonly string airlineCode;

        //List that holds all the Flight Numbers 
        /// so we won't reuse the same flight number every day
        private List<int> flightNumbers;

        //List of all the planes that have been created for this airline
        private List<Plane> planesCreated;

        //home airport for airline
        //some airlines have home airport like EL AL and Ben Gurion Airport
        private int homeAirport;

        /// <summary>
        /// Constructor for the Airline class
        /// </summary>
        /// <param name="airlineID"></param>
        /// <param name="airlineName"></param>
        /// <param name="airlineCode"></param>
        /// <param name="allPlanesList"></param>
        /// <param name="homeAirport"></param>
        public Airline(int airlineID, string airlineName, string airlineCode, List<PlaneQuantityManager> allPlanesList, int homeAirport)
        {
            this.airlineID = airlineID;
            this.airlineName = airlineName;
            this.airlineCode = airlineCode;
            this.planesCreated = new List<Plane>();
            this.flightNumbers = new List<int>();
            this.homeAirport = homeAirport;
        }

        /// <summary>
        /// Get AirlineID
        /// </summary>
        /// <returns></returns>
        public int GetAirlineID()
        {
            return airlineID;
        }

        /// <summary>
        /// Get Airline Name
        /// </summary>
        /// <returns></returns>
        public string GetAirlineName()
        {
            return this.airlineName;
        }

        /// <summary>
        /// Get Airline code the airline code.
        /// acts as the individual code for an airline
        /// the code will be used for the
        /// </summary>
        /// <returns></returns>
        public string GetAirlineCode()
        {
            return this.airlineCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetFlightNumbers()
        {
            return this.flightNumbers;
        }

        /// <summary>
        /// Add flight number for the flight numbers List
        /// </summary>
        /// <param name="nums"></param>
        public void AddFlightNumbers(int nums)
        {
            this.flightNumbers.Add(nums);
        }

        /// <summary>
        /// Get home airport
        /// </summary>
        /// <returns></returns>
        public int GetHomeAirport()
        {
            return this.homeAirport;
        }


        /// <summary>
        /// Bind plane created to its airline
        /// </summary>
        /// <param name="plane"></param>
        public void BindPlaneToAirline(Plane plane)
        {
            planesCreated.Add(plane);
            plane.BindAirlineToCurrentPlane(this, planesCreated.IndexOf(plane));
        }
    }
}