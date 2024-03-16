﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using UnityEditor;

namespace Assets
{
    public class Airline
    {
        private readonly int airlineID;
        private readonly string airlineName;
        private readonly string airlineCode;
        private List<int> flightNumbers;
        //private List<PlaneQuantityManager> allPlanesList;
        private List<Plane> PlanesCreated;
        private int homeAirport;

        public Airline(int airlineID, string airlineName, string airlineCode, List<PlaneQuantityManager> allPlanesList, int homeAirport)
        {
            this.airlineID = airlineID;
            this.airlineName = airlineName;
            this.airlineCode = airlineCode;
            //this.allPlanesList = allPlanesList;
            this.PlanesCreated = new List<Plane>();
            this.flightNumbers = new List<int>();
            this.homeAirport = homeAirport;
        }

        public int GetAirlineID()
        {
            return airlineID;
        }

        public string GetAirlineName()
        {
            return this.airlineName;
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

        public int GetHomeAirport()
        {
            return this.homeAirport;
        }

        //public List<PlaneQuantityManager> GetPlanesList()
        //{
        //    return this.allPlanesList;
        //}

        //public static PlaneQuantityManager Get 

        public void BindPlaneToAirline(Plane plane)
        {
            PlanesCreated.Add(plane);
            plane.BindAirlineToCurrentPlane(this, PlanesCreated.IndexOf(plane));
        }

        //public List<int> GetAirportsFlyingTo()
        //{
        //    return this.airportsFlyingTo;
        //}
    }
}