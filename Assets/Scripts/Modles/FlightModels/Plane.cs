using Assets.Scripts.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.Rendering;
using UnityEngine;

namespace Assets
{
    public class Plane
    {
        private string realPlaneID;
        private readonly int DBplaneID;
        private readonly string planeName;
        private readonly int fuelCapacity;
        private int fuelDropRate;
        private int currentFuelLevel;
        private Airline airline;
        private Flight flight;
        private int avrSpeed;
        private int maxSpeed;
        private int currentSpeed;
        private int maxRange;
        private int grade;

        public Plane(int DBplaneID, string planeName, int fuelCapacity, int fuelDropRate, int avrSpeed, int maxSpeed, int maxRange, int grade)
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
            this.currentSpeed = this.avrSpeed;
            this.flight = null;
            this.maxRange = maxRange;
            this.grade = grade;
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

        public int CalculateMaxSpeedIncrease(DateTime currentTime)
        {
            int minFuelPossible = (int)(this.fuelDropRate * SimulationController.minFuelPercentageAllowedAtLanding);
            int newFuelDropRate = (int)(this.fuelDropRate * SimulationController.fuelBurnRateDiffCruiseToMax);
            int distanceLeft = this.flight.GetDistanceToArrivalAirport(currentTime);

            if (this.currentFuelLevel - minFuelPossible <= 0)
            {
                UnityEngine.Debug.Log("Error in Fuel LEVEL");
                UnityEngine.Debug.Log(this.DBplaneID + " " + this.planeName);
                return -1;
            }

            int newSpeed = (newFuelDropRate * distanceLeft) / (minFuelPossible - GetCurrentFuelLevel(currentTime));

            if (newSpeed <= this.avrSpeed)
                return -1;

            else if(newSpeed <0 )
                return avrSpeed;

            return Math.Min(newSpeed, this.maxSpeed);
        }

        public int GetPlaneID()
        {
            return this.DBplaneID;
        }

        public int GetAvrSpeed()
        {
            return this.avrSpeed;
        }

        public int GetCurrentSpeed()
        {
            return this.currentSpeed;
        }

        public int GetGrade()
        {
            return this.grade;
        }

        public int GetFuelDropRate()
        {  
            return this.fuelDropRate; 
        }   

        public int GetCurrentFuelLevel(DateTime currentTime)
        {
            this.currentFuelLevel = this.fuelCapacity - (this.GetFuelDropRate() * this.flight.GetTimeTraveledMin(currentTime))/60;
            return this.currentFuelLevel;
        }

        public void SetNewCurrentFuelLevel(int fuelLevel)
        {
            this.currentFuelLevel = fuelLevel;
        }

        public void SetNewFuelDropRate(int fuelDropRate)
        {
            this.fuelDropRate = fuelDropRate;
        }

        public void SetCurrentSpeed(int AvrSpeed)
        {
            this.currentSpeed = AvrSpeed;
        }

        public void SetFlight(Flight flight)
        {
            if ( flight != null )
            {
                this.flight = flight;
            }
        }

        public int GetMaxDistanceAvailable(DateTime currentTime)
        {
            double distance = ((double)GetCurrentFuelLevel(currentTime) / this.fuelDropRate) * this.currentSpeed;
            return (int)distance;
        }

        public override string ToString()
        {
            return this.planeName;
        }
    }
}