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
        private string status;
        private readonly bool isLandingAtMain;
        private bool landed;
        private bool isTakeoff;

        public Flight(Airline airline, Plane plane, Airport departingAirport, Airport arrivalAirport, DateTime estimatedTakeoffTime, int runway)
        {
            this.airline = airline;
            this.plane = plane;
            this.departingAirport = departingAirport;
            this.arrivalAirport = arrivalAirport;
            this.flightDistance = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(this.departingAirport.GetLatitude(), this.departingAirport.GetLongitude(),
                                                                                           this.arrivalAirport.GetLatitude(), this.arrivalAirport.GetLongitude());
            this.estimatedTakeoffTime = estimatedTakeoffTime;
            EstimatedLandingTime();
            this.problem = null;
            this.location = departingAirport;
            this.runway = runway;
            this.isLandingAtMain = IsLandingOrTakeOffFlight();

            this.distanceTraveled = 0;
            this.landed = false;
            this.isTakeoff = false;
            this.status = "Waiting";

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

        public string GetFlightNumber()
        {
            return this.flightNumber;
        }

        public int GetRunway()
        {
            return this.runway;
        }

        public void SetRunway(int runway)
        {
            this.runway = runway;
        }

        public void EstimatedLandingTime()
        {
            try
            {
                this.flightDuration = Math.Abs(CalculateFlightDuration());
                TimeSpan duration = TimeSpan.FromMinutes(this.flightDuration);
                this.estimatedLandingTime = this.estimatedTakeoffTime.Add(duration);
            }

            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("flightDuration: " + this.flightDuration);
                Debug.Log("plane: " + this.plane.ToString());
                Debug.Log("planeAVRSpeed: " + this.plane.GetAvrSpeed());
                Debug.Log("flightDistance: " + this.flightDistance);
            }
        }

        public int GetTimeTraveledMin(DateTime currentTime)
        {
            TimeSpan timeElapsed = currentTime - this.estimatedTakeoffTime;
            return (int)Math.Round(timeElapsed.TotalMinutes);
        }

        public bool DidFlightTakeoff()
        {
            return this.isTakeoff;
        }

        public void FlightTookOff()
        {
            this.status = "In Air";
            this.isTakeoff = true;
        }
 
        public void FlightLanded()
        {
            this.status = this.status != "In Air" ? ": " + this.status : "";
            this.status = "Landed" + this.status;
            this.landed = true;
        }

        public bool FlightHasLanded()
        {
            return this.landed;
        }

        public string GetStatus()
        {
            return this.status;
        }

        public int GetDistanceTraveled(DateTime currentTime)
        {
            // Dividing by 60.0 ensures floating-point division
            double distanceTraveled = this.plane.GetAvrSpeed() * (GetTimeTraveledMin(currentTime) / 60.0);
            this.distanceTraveled =  (int)Math.Round(distanceTraveled);
            return this.distanceTraveled;
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
        /// Get the landing time of the flight
        /// </summary>
        /// <returns></returns>
        public DateTime GetLandingTime()
        {
            return this.estimatedLandingTime;
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

        public void ChangeEitherTime(int newTime)
        {
            if (IsLandingOrTakeOffFlight())
            {
                ChangeLandingTime(newTime);
                return;
            }

            ChangeTakeoffTime(newTime);
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

        public void SetStatus(String status)
        {
            this.status = status;
        }

        public void SetEstimatedLandingTime(DateTime landingTime)
        {
            this.estimatedLandingTime = landingTime;
        }

        public Airport GetArrivalAirport()
        {
            return this.arrivalAirport;
        }

        public Airport GetDepartingAirport()
        {
            return this.departingAirport;
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

            else if(str1 == "completed")
            {
                str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
                str += " - " + this.airline.GetAirlineName() + " - " + this.status;
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