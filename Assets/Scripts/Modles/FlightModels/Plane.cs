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

        /// <summary>
        /// Create the Plane ID
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string CreatePlaneID(int index)
        {
            return this.DBplaneID.ToString() + this.airline.GetAirlineCode().ToString() + '_' + index.ToString();
        }
        
        /// <summary>
        /// Bind the plane to the airline
        /// </summary>
        /// <param name="airline"></param>
        /// <param name="index"></param>
        public void BindAirlineToCurrentPlane(Airline airline, int index)
        {
            if (this.airline != null)
            {
                this.airline = airline;
                this.realPlaneID = CreatePlaneID(index);
            }
        }

        /// <summary>
        /// Calculate the max speed increase possible
        /// used to calculate the max speed possible to increase for a flight with an issue
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the plane Id
        /// </summary>
        /// <returns></returns>
        public int GetPlaneID()
        {
            return this.DBplaneID;
        }

        /// <summary>
        /// Get the average speed of the flight
        /// </summary>
        /// <returns></returns>
        public int GetAvrSpeed()
        {
            return this.avrSpeed;
        }

        /// <summary>
        /// Get the current speed of the flight
        /// </summary>
        /// <returns></returns>
        public int GetCurrentSpeed()
        {
            return this.currentSpeed;
        }

        /// <summary>
        /// Get the grade of the plane 
        /// equivillant to the airport runway grade
        /// </summary>
        /// <returns></returns>
        public int GetGrade()
        {
            return this.grade;
        }

        /// <summary>
        /// Get the fuel drop rate
        /// </summary>
        /// <returns></returns>
        public int GetFuelDropRate()
        {  
            return this.fuelDropRate; 
        }   

        /// <summary>
        /// Get the current fuel level
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public int GetCurrentFuelLevel(DateTime currentTime)
        {
            this.currentFuelLevel = this.fuelCapacity - (this.GetFuelDropRate() * this.flight.GetTimeTraveledMin(currentTime))/60;
            return this.currentFuelLevel;
        }

        /// <summary>
        /// Set a new Fuel level
        /// </summary>
        /// <param name="fuelLevel"></param>
        public void SetNewCurrentFuelLevel(int fuelLevel)
        {
            this.currentFuelLevel = fuelLevel;
        }

        /// <summary>
        /// Set new Fuel Drop Rate
        /// </summary>
        /// <param name="fuelDropRate"></param>
        public void SetNewFuelDropRate(int fuelDropRate)
        {
            this.fuelDropRate = fuelDropRate;
        }

        /// <summary>
        /// Set the current speed of the flight
        /// </summary>
        /// <param name="AvrSpeed"></param>
        public void SetCurrentSpeed(int AvrSpeed)
        {
            this.currentSpeed = AvrSpeed;
        }

        /// <summary>
        /// set the flight that the plane is currently on
        /// </summary>
        /// <param name="flight"></param>
        public void SetFlight(Flight flight)
        {
            if ( flight != null )
            {
                this.flight = flight;
            }
        }

        /// <summary>
        /// Calculate the maximum flight the plane can travel
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public int GetMaxDistanceAvailable(DateTime currentTime)
        {
            double distance = ((double)GetCurrentFuelLevel(currentTime) / this.fuelDropRate) * this.currentSpeed;
            return (int)distance;
        }

        /// <summary>
        /// to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.planeName;
        }
    }
}