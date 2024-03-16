using Assets.Scripts.Modles.IssuesControler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets
{
    public class Flight
    {
        private int flightID;
        private string flightNumber;
        private Airline airline;
        private Plane plane;
        private Airport departingAirport;
        private Airport arrivalAirport;
        private ProblemCreator problem;
        private int flightDistance;
        private int distanceTraveled;
        private DateTime estimatedTakeoffTime;
        private DateTime estimatedLandingTime;
        private int flightDuration;
        private Location location;
        private int timeInAir;
        private bool isReady;
        private bool Landed;
        private bool isLanding;
        private bool isLandingPermission;
        private bool isTakeoff;
        private bool isTakeoffPermission;
        private bool hasEmergency;
        private bool isOnRunway;
        private bool isOnRunwayPermission;
        private bool isOnTaxiway;
        private bool isOnTaxiPermission;
        private bool isDelayed;
        private bool isDelayInternal;
        private bool isDelayExternal;
        private bool isCancled;
        private bool planeInUse;
        public static int NumberOfFlights = 0;

        public Flight(Airline airline, Plane plane, Airport departingAirport, Airport arrivalAirport, DateTime estimatedTakeoffTime)
        {
            this.flightID = ++NumberOfFlights;
            this.airline = airline;
            this.plane = plane;
            this.departingAirport = departingAirport;
            this.arrivalAirport = arrivalAirport;
            this.flightDistance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.departingAirport.GetLatitude(), this.departingAirport.GetLongitude(),
                                                                       this.arrivalAirport.GetLatitude(), this.arrivalAirport.GetLongitude());
            this.estimatedTakeoffTime = estimatedTakeoffTime;
            this.flightDuration = CalculateFlightDuration();
            this.estimatedLandingTime = EstimatedLandingTime();
            this.problem = null;
            this.location = departingAirport;

            this.timeInAir = 0;
            this.distanceTraveled = 0;
            this.Landed = false;
            this.isReady = false;
            this.isLanding = false;
            this.isLandingPermission = false;
            this.isTakeoff = false;
            this.isTakeoffPermission = false;
            this.hasEmergency = false;
            this.isOnRunway = false;
            this.isOnRunwayPermission = false;
            this.isOnTaxiway = false;
            this.isOnTaxiPermission = false;
            this.isDelayed = false;
            this.isDelayInternal = false;
            this.isDelayExternal = false;
            this.isCancled = false;

            this.flightNumber = CreateFlightNumber();
            BindPlaneToFlight();
        }

        private string CreateFlightNumber()
        {
            System.Random rnd = new System.Random();
            bool flag = true;
            int flight_numbers = 0;
            while (flag)
            {
                flight_numbers = rnd.Next(100, 1000);
                flag = false;

                foreach (int combination in this.airline.GetFlightNumbers())
                {
                    if (combination == flight_numbers)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            this.airline.AddFlightNumbers(flight_numbers);
            return this.airline.GetAirlineCode() + flight_numbers;
        }

        private int CalculateFlightDuration()
        {
            double ETA = this.flightDistance / ((double)this.plane.GetAvrSpeed());
            ETA += 0.75;
            ETA *= 60;

            return (int)ETA;
        }

        private void BindPlaneToFlight()
        {
            this.plane.SetFlight(this);
        }

        public void AddProblemToFlight(ProblemCreator problem)
        {
            this.problem = problem;
        }

        private Location GetFlightLocation()
        {
            if (!this.isTakeoff)
            {
                this.location = this.departingAirport;
            }

            else if (this.Landed)
            {
                this.location = this.arrivalAirport;
            }

            else
            {
                this.location = DistanceAndLocationsFunctions.GetCoorWithBearingAndDistance(this.departingAirport.GetLatitude(), this.departingAirport.GetLongitude(),
                                                                    this.arrivalAirport.GetLatitude(), this.arrivalAirport.GetLongitude(), GetDistanceTravled());
            }

            return this.location;
        }

        private DateTime EstimatedLandingTime()
        {
            return this.estimatedTakeoffTime.AddMinutes(this.flightDuration);
        }

        public int GetDistanceTravled()
        {
            this.distanceTraveled = this.timeInAir * this.plane.GetAvrSpeed();
            return this.distanceTraveled;
        }

        /// <summary>
        /// returns the flight duration in minutes
        /// </summary>
        /// <returns></returns>
        public int GetFlightDurationMinutes()
        {
            return this.flightDuration;
        }

        /// <summary>
        /// returns the flight duration in hours
        /// </summary>
        /// <returns></returns>
        public int GetFlightDurationHours()
        {
            return this.flightDuration/60;
        }

        public DateTime GetEstimatedLanding()
        {
            return this.estimatedLandingTime;
        }

        /// <summary>
        /// Get The takeof time of the flight
        /// </summary>
        /// <returns></returns>
        public DateTime GetTakeOffTime()
        {
            return this.estimatedTakeoffTime;
        }

        /// <summary>
        /// Returns the problem if the flight has one
        /// otherwise null
        /// </summary>
        /// <returns></returns>
        public ProblemCreator GetProblem()
        {
            return this.problem;
        }

        public string ToString(string str1)
        {
            string str = "";

            if (str1 == "HH:mm")
            {
                str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
                str += " - " + this.airline.GetAirlineName() + " - " + estimatedTakeoffTime.ToString("HH:mm");
            }

            return str;
        }

        public new string ToString()
        {
            string str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
            str += " - " + this.airline.GetAirlineName() + " - " + estimatedTakeoffTime.ToString("HH:mm dd-MM-yyyy");
            return str;
        }
    }
}