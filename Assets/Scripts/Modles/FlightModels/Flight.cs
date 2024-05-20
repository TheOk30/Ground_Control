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
    /// <summary>
    /// Class that holds the information about the flight
    /// </summary>
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

        /// <summary>
        /// Function that creates a flight number for the flight
        /// if the flight number is already in use it will create a new one
        /// </summary>
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
                        break;
                    }
                }
            }

            this.airline.AddFlightNumbers(flight_numbers);
            this.flightNumber = this.airline.GetAirlineCode() + flight_numbers;
        }

        /// <summary>
        /// Calculate the flight duration in minutes
        /// </summary>
        /// <returns></returns>
        private int CalculateFlightDuration()
        {
            double ETA = this.flightDistance / ((double)this.plane.GetAvrSpeed());
            ETA += 0.75;
            ETA *= 60;

            return (int)ETA;
        }

        /// <summary>
        /// Bind the plane to the flight
        /// </summary>
        private void BindPlaneToFlight()
        {
            this.plane.SetFlight(this);
        }

        /// <summary>
        /// Add the problem to the flight
        /// In every flight there is a chance that a problem will occur
        /// acts as a black box for the flight
        /// </summary>
        /// <param name="problem"></param>
        public void AddProblemToFlight(ProblemCreator problem)
        {
            this.problem = problem;
        }

        /// <summary>
        /// Get the flight number
        /// </summary>
        /// <returns></returns>
        public string GetFlightNumber()
        {
            return this.flightNumber;
        }

        /// <summary>
        /// Get the runway the flight is taking off /
        /// </summary>
        /// <returns></returns>
        public int GetRunway()
        {
            return this.runway;
        }

        /// <summary>
        /// Set the Runway the flight is taking off or 
        /// land on based on the reordering of the flights
        /// </summary>
        /// <param name="runway"></param>
        public void SetRunway(int runway)
        {
            this.runway = runway;
        }

        /// <summary>
        /// Get the estimated landing time of the flight
        /// </summary>
        public void EstimatedLandingTime()
        {
            try
            {
                this.flightDuration = Math.Abs(CalculateFlightDuration());
                TimeSpan duration = TimeSpan.FromMinutes(this.flightDuration);
                this.estimatedLandingTime = this.estimatedTakeoffTime.Add(duration);
                this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(-this.estimatedLandingTime.Second);
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

        /// <summary>
        /// Get the time the flight has traveled in minutes
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public int GetTimeTraveledMin(DateTime currentTime)
        {
            TimeSpan timeElapsed = currentTime - this.estimatedTakeoffTime;
            return (int)Math.Round(timeElapsed.TotalMinutes);
        }

        /// <summary>
        /// Did the flight take off
        /// </summary>
        /// <returns></returns>
        public bool DidFlightTakeoff()
        {
            return this.isTakeoff;
        }

        /// <summary>
        /// change flight status to in air
        /// after the flight has taken off
        /// </summary>
        public void FlightTookOff()
        {
            this.status = "In Air";
            this.isTakeoff = true;
        }
 
        /// <summary>
        /// Flight has landed
        /// </summary>
        public void FlightLanded()
        {
            this.status = this.status != "In Air" ? ": " + this.status : "";
            this.status = "Landed" + this.status;
            this.landed = true;
            AirportManager.Instance.GetFlightSchedule().GetFlights().RemoveNode(this);
            AirportManager.Instance.AddFlightToLandedFlights(this);
        }

        /// <summary>
        /// check if flight has landed
        /// </summary>
        /// <returns></returns>
        public bool FlightHasLanded()
        {
            return this.landed;
        }

        /// <summary>
        /// Get the status of the flight
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            return this.status;
        }

        /// <summary>
        /// Get the distance traveled by the flight up to the current system time
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public int GetDistanceTraveled(DateTime currentTime)
        {
            // Dividing by 60.0 ensures floating-point division
            double distanceTraveled = this.plane.GetAvrSpeed() * (GetTimeTraveledMin(currentTime) / 60.0);
            this.distanceTraveled =  (int)Math.Round(distanceTraveled);
            return this.distanceTraveled;
        }

        /// <summary>
        /// Check if the flight is landing or taking off at the main airport
        /// </summary>
        /// <returns></returns>
        public bool IsLandingOrTakeOffFlight()
        {
            return this.arrivalAirport.GetAirportCode() == AirportManager.Instance.GetMainAirport().GetAirportCode();
        }

        /// <summary>
        /// Get the current location of the flight
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the distance to the arrival airport
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public int GetDistanceToArrivalAirport(DateTime currentTime)
        {
            return this.arrivalAirport.DistanceFromCurrentAirport(GetFlightLocation(currentTime));
        }

        /// <summary>
        /// Get the plane the flight is using
        /// </summary>
        /// <returns></returns>
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
        /// Get the estimated landing time of the flight
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Is the flight landing at the main airport
        /// </summary>
        /// <returns></returns>
        public bool GetIsFlightLandingAtMain()
        {
            return this.isLandingAtMain;
        }

        /// <summary>
        /// Change the time of the flight 
        /// relevent to the main airport
        /// </summary>
        /// <param name="newTime"></param>
        public void ChangeEitherTime(int newTime)
        {
            if (IsLandingOrTakeOffFlight())
            {
                ChangeLandingTime(newTime);
                return;
            }

            ChangeTakeoffTime(newTime);
        }

        /// <summary>
        /// Change the takeoff time of the flight
        /// </summary>
        /// <param name="add"></param>
        public void ChangeTakeoffTime(int add)
        {
            this.estimatedTakeoffTime = this.estimatedTakeoffTime.AddSeconds(add);
            this.estimatedTakeoffTime = this.estimatedTakeoffTime.AddSeconds(-this.estimatedTakeoffTime.Second);
            this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(add);
            this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(-this.estimatedLandingTime.Second);
        }

        /// <summary>
        /// Change the landing time of the flight
        /// </summary>
        /// <param name="add"></param>
        public void ChangeLandingTime(int add)
        {
            this.flightDuration += add;
            this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(add);
            this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(-this.estimatedLandingTime.Second);
        }

        /// <summary>
        /// Set the status of the flight
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(string status)
        {
            this.status = status;
        }

        /// <summary>
        /// Set the estimated landing time of the flight
        /// </summary>
        /// <param name="landingTime"></param>
        public void SetEstimatedLandingTime(DateTime landingTime)
        {
            this.estimatedLandingTime = landingTime;
            this.estimatedLandingTime = this.estimatedLandingTime.AddSeconds(-this.estimatedLandingTime.Second);
        }

        /// <summary>
        /// Get the arrival airport
        /// </summary>
        /// <returns></returns>
        public Airport GetArrivalAirport()
        {
            return this.arrivalAirport;
        }

        /// <summary>
        /// Get the departing airport
        /// </summary>
        /// <returns></returns>
        public Airport GetDepartingAirport()
        {
            return this.departingAirport;
        }

        /// <summary>
        /// Get the time to compare for the flight to be sorted
        /// for the Icompare interface
        /// </summary>
        /// <returns></returns>
        public DateTime GetTimeToCompare()
        {
            return this.isLandingAtMain ? this.estimatedLandingTime : this.estimatedTakeoffTime;
        }

        /// <summary>
        /// Set the alternate arrival airport
        /// </summary>
        /// <param name="alternateAirport"></param>
        public void SetAlternateArrivalAirport(Airport alternateAirport)
        {
            this.arrivalAirport = alternateAirport;
        }

        /// <summary>
        /// CompareTo method to compare each flight based on the time
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Flight other)
        {
            return this.GetTimeToCompare().CompareTo(other.GetTimeToCompare());
        }

        /// <summary>
        /// To string Method with a format
        /// </summary>
        /// <param name="str1"></param>
        /// <returns></returns>
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

        /// <summary>
        /// regular to string method
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            string str = this.flightNumber + ": " + this.departingAirport.GetAirportCode() + " -> " + this.arrivalAirport.GetAirportCode();
            str += " - " + this.airline.GetAirlineName() + " - " + estimatedTakeoffTime.ToString("HH:mm dd-MM-yyyy") + " -> " + estimatedLandingTime.ToString("HH:mm dd-MM-yyyy");
            return str;
        }
    }
}