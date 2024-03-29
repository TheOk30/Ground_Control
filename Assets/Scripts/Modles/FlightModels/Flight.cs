using Assets.Scripts.Controller;
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
    public class Flight : IComparable<Flight>
    {
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
        private int runway;

        private readonly bool isLandingAtMain;
        private bool isReady;
        private bool landed;
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

        public Flight(Airline airline, Plane plane, Airport departingAirport, Airport arrivalAirport, DateTime estimatedTakeoffTime, int runway)
        {
            this.airline = airline;
            this.plane = plane;
            this.departingAirport = departingAirport;
            this.arrivalAirport = arrivalAirport;
            this.flightDistance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.departingAirport.GetLatitude(), this.departingAirport.GetLongitude(),
                                                                                           this.arrivalAirport.GetLatitude(), this.arrivalAirport.GetLongitude());
            this.estimatedTakeoffTime = estimatedTakeoffTime;
            CalculateFlightDuration();
            EstimatedLandingTime();
            this.problem = null;
            this.location = departingAirport;
            this.runway = runway;
            this.isLandingAtMain = IsLandingOrTakeOffFlight();

            this.distanceTraveled = 0;
            this.landed = false;
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

            CreateFlightNumber();
            BindPlaneToFlight();
        }

        private void CreateFlightNumber()
        {
            bool flag = true;
            int flight_numbers = 0;
             
            while (flag)
            {
                flight_numbers = SimulationController.rnd.Next(100, 1000);
                flag = false;

                foreach (int combination in this.airline.GetFlightNumbers())
                {
                    if (combination == flight_numbers)
                    {
                        flag = true;
                    }
                }
            }

            this.airline.AddFlightNumbers(flight_numbers);
            this.flightNumber = this.airline.GetAirlineCode() + flight_numbers;
        }

        private void CalculateFlightDuration()
        {
            double ETA = this.flightDistance / ((double)this.plane.GetAvrSpeed());
            ETA += 0.75;
            ETA *= 60;

            this.flightDuration = (int)ETA;
        }

        private void BindPlaneToFlight()
        {
            this.plane.SetFlight(this);
        }

        public void AddProblemToFlight(ProblemCreator problem)
        {
            this.problem = problem;
        }

        private void EstimatedLandingTime()
        {
            TimeSpan duration = TimeSpan.FromMinutes(this.flightDuration);
            this.estimatedLandingTime = this.estimatedTakeoffTime.Add(duration);
        }

        public int GetTimeTraveledMin(DateTime currentTime)
        {
            TimeSpan timeElapsed = currentTime - this.estimatedTakeoffTime;
            return (int)Math.Round(timeElapsed.TotalMinutes);
        }

        public int GetDistanceTraveled(DateTime currentTime)
        {
            // Dividing by 60 to convert speed to minutes
            double distanceTraveled = this.plane.GetAvrSpeed() * (GetTimeTraveledMin(currentTime) / 60); 
            return (int)Math.Round(distanceTraveled);
        }

        public bool IsLandingOrTakeOffFlight()
        {
            return this.arrivalAirport.GetAirportCode() == AirportManager.Instance.GetMainAirport().GetAirportCode();
        }

        public Location GetFlightLocation(DateTime currentTime)
        {
            if (!this.isTakeoff)
            {
                this.location = this.departingAirport;
            }

            else if (this.landed)
            {
                this.location = this.arrivalAirport;
            }

            else
            {
                this.location = DistanceAndLocationsFunctions.GetCoorWithBearingAndDistance(this.departingAirport.GetLatitude(), this.departingAirport.GetLongitude(),
                                                                    this.arrivalAirport.GetLatitude(), this.arrivalAirport.GetLongitude(), GetDistanceTraveled(currentTime));
            }

            return this.location;
        }

        public int GetDistanceToArrivalAirport(DateTime currentTime)
        {
            return this.arrivalAirport.DistanceFromCurrentAirport(GetFlightLocation(currentTime));
        }

        public Plane GetPlane()
        {
            return this.plane;
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

        public bool GetIsFlightLandingAtMain()
        {
            return this.isLandingAtMain;
        }

        public void ChangeTakeoffTime(int add)
        {
            this.estimatedTakeoffTime.AddMinutes(add);
            this.estimatedLandingTime.AddMinutes(add);
        }

        public void ChangeLandingTime(int add)
        {
            this.flightDuration += add;
            this.estimatedLandingTime.AddMinutes(add);
        }

        public void SetEstimatedLandingTime(DateTime landingTime)
        {
            this.estimatedLandingTime = landingTime;
        }

        public Airport GetArrivalAirport()
        {
            return this.GetArrivalAirport();
        }

        public DateTime GetTimeToCompare()
        {
            return this.isLandingAtMain ? this.estimatedLandingTime : this.estimatedTakeoffTime;
        }

        public void SetAlternateArrivalAirport(Airport alternateAirport)
        {
            this.arrivalAirport = alternateAirport;
        }

        public int CompareTo(Flight other)
        {
            return this.GetTimeToCompare().CompareTo(other.GetTimeToCompare());
        }

        public string ToString(string str1)
        {
            string str = "";

            if (str1 == "HH:mm")
            {
                str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
                str += " - " + this.airline.GetAirlineName() + " - " + estimatedTakeoffTime.ToString("HH:mm") + " - " + estimatedLandingTime.ToString("HH:mm");
            }

            return str;
        }

        public new string ToString()
        {
            string str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
            str += " - " + this.airline.GetAirlineName() + " - " + estimatedTakeoffTime.ToString("HH:mm dd-MM-yyyy") + " -> " + estimatedLandingTime.ToString("HH:mm dd-MM-yyyy");
            return str;
        }
    }
}