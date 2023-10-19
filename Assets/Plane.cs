using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Rendering;

namespace Assets
{
    class Plane
    {
        private readonly string realPlaneID;
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
        private int distanceTraveled;
        public static int AlldistanceTraveled = 0;

        public Plane(int DBplaneID, string planeName, int fuelCapacity, Airline airline, int avrSpeed, int maxSpeed, int fuelDropRate)
        {
            this.DBplaneID = DBplaneID;
            this.realPlaneID = createPlaneID();
            this.planeName = planeName;
            this.fuelCapacity = fuelCapacity;
            this.currentFuelLevel = fuelCapacity;
            this.fuelDropRate = fuelDropRate;
            this.airline = airline;
            this.avrSpeed = avrSpeed;
            this.maxSpeed = maxSpeed;
            this.currentSpeed = 0;
            this.distanceTraveled = 0;
            this.flight = null;
        }

        private string createPlaneID()
        {

        }

        public int GetAvrSpeed( )
        {
            return this.avrSpeed;
        }

        public bool SetFlight(Flight flight)
        {
            if ( flight == null )
            {
                this.flight = flight;
                return true;
            }

            return false;
        }
    }
}
