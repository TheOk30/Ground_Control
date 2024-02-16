using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Rendering;

namespace Assets
{
    public class Plane
    {
        private string realPlaneID;
        private readonly int DBplaneID;
        private readonly string planeName;
        private readonly int fuelCapacity;
        private readonly int fuelDropRate;
        private int currentFuelLevel;
        private Airline airline;
        private Flight flight;
        private int avrSpeed;
        private int maxSpeed;
        private int currentSpeed;
        private int maxRange;
        private int distanceTraveled;
        private bool inUse;
        public static int AlldistanceTraveled = 0;

        public Plane(int DBplaneID, string planeName, int fuelCapacity, int fuelDropRate, int avrSpeed, int maxSpeed, int maxRange)
        {
            this.DBplaneID = DBplaneID;
            this.realPlaneID = "";
            this.planeName = planeName;
            this.fuelCapacity = fuelCapacity;
            this.currentFuelLevel = fuelCapacity;
            this.fuelDropRate = fuelDropRate;
            this.airline = null;
            this.avrSpeed = avrSpeed;
            this.maxSpeed = maxSpeed;
            this.currentSpeed = 0;
            this.distanceTraveled = 0;
            this.flight = null;
            this.inUse = false;
            this.maxRange = maxRange;
        }

        private string CreatePlaneID(int index)
        {
            return this.DBplaneID.ToString() + this.airline.GetAirlineCode().ToString() + '_' + index.ToString();
        }
        
        public void BindAirlineToCurrentPlane(Airline airline, int index)
        {
            if (this.airline != null)
            {
                this.airline = airline;
                this.realPlaneID = CreatePlaneID(index);
            }
        }

        public int GetPlaneID()
        {
            return this.DBplaneID;
        }

        public int GetAvrSpeed()
        {
            return this.avrSpeed;
        }

        public bool isInUse()
        {
            return this.inUse;
        }

        //public DateTime WhenWillBeReady()
        //{
        //    TimeSpan timeSpan = TimeSpan.FromHours(flight.GetFlightDuration()+1);
        //     return flight.GetEstimatedLanding().Add(timeSpan);
        //}
        public void SetFlight(Flight flight)
        {
            if ( flight != null )
            {
                this.flight = flight;
            }
        }
    }
}
