using Assets.Scripts.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static Assets.FlightIssues;


namespace Assets.Scripts.SolverController
{
    public class Solver
    {
        //Singleton Instance for solver class
        public static Solver Instance = null;

        //Delegate for the functions to add them to the dictionary
        private delegate void SolvingFunctions(Flight flight, DateTime currTime);

        private Dictionary<int, SolvingFunctions> solvingFunctionsDict;

        private static SpatialHashingLocationIndex spatialHashLocation;

        // Define an array of functions for each solving function
        private SolvingFunctions[] functionArray = new SolvingFunctions[]
        {
            LightlySickSolver,
            OtherSevereSickSolver,
            LightlyWoundedSolver,
            SeverelyWoundedSolver,
            DyingSolver,
            NotEnoughFuelSolver,
            FasterBurnRateSolver,
            BrokenWheelsSolver,
            ThrustIssueSolver,
            LooseDoorSolver,
            MissingDoorSolver,
            BrakeIssueSolver,
            LowOutputSingleSolver,
            LowOutputDoubleSolver,
            EngineFailureSingleSolver,
            EngineFailureDoubleSolver
        };

        /// <summary>
        /// Solver Class Singleton Intiatied by the TimerUI class
        /// </summary>
        private Solver()
        {
            // Create a dictionary to hold enum values and their corresponding arrays of functions
            solvingFunctionsDict = new Dictionary<int, SolvingFunctions>();

            // Populate the dictionary with arrays of functions for each enum value
            for (int i = 0; i < functionArray.Length; i++)
            {
                solvingFunctionsDict.Add(i + 1, functionArray[i]);
            }

            spatialHashLocation = SpatialHashingLocationIndex.InitializeSolver();
        }

        /// <summary>
        /// Fuction Gets flight and invokes the corresponding solving function from the dictionary
        /// </summary>
        /// <param name="flight"></param>
        public void SolveFlightIssue(Flight flight)
        {
            DateTime currTime = Timer.time;
            solvingFunctionsDict[flight.GetProblem().GetIssue().GetId()].Invoke(flight, currTime);

            Debug.Log(Timer.time + " takeoff: " + flight.GetDepartingAirport() + " landing: " + 
                    flight.GetArrivalAirport() + " current location: " + flight.GetFlightLocation(Timer.time));
        }

        /// <summary>
        /// Reorder the flight schedule priority queue
        /// </summary>
        private static void ReorderFlightsSchedule(Flight flightToReorder)
        {
            Debug.Log("Reordering Flights Schedule");
            Debug.Log("Reordering " + flightToReorder.GetFlightNumber());

            if (!flightToReorder.IsLandingOrTakeOffFlight())
                return;

            int flightIndex = AirportManager.Instance.GetFlightSchedule().GetFlights().GetHeap().IndexOf(flightToReorder);
            AirportManager.Instance.GetFlightSchedule().GetFlights().HeapifyUp(flightIndex);
            
            bool flag = false;
            while (!flag)
            {
                flag = true;
                List<Flight> flightsList = AirportManager.Instance.GetFlightSchedule().GetFlights().GetSortedWithoutModifyingHeap();
                DateTime[] DifferentLaneUseTime = new DateTime[AirportManager.Instance.GetNumRunways()];
                int[] LastUsedIndexPerLane = new int[AirportManager.Instance.GetNumRunways()];

                for (int i = 0; i < DifferentLaneUseTime.Length; i++)
                {
                    DifferentLaneUseTime[i] =  DateTime.MinValue;
                }

                DifferentLaneUseTime[flightsList[0].GetRunway() - 1] = flightsList[0].GetTimeToCompare();
                LastUsedIndexPerLane[flightsList[0].GetRunway() - 1] = 0;

                for (int i = 1; i < flightsList.Count; i++)
                {
                    int index = i;

                    TimeSpan difference;
                    if (DifferentLaneUseTime.Min() != DateTime.MinValue)
                        difference = flightsList[i].GetTimeToCompare() - DifferentLaneUseTime.Min();

                    else
                        difference = TimeSpan.FromSeconds((SimulationController.TimeBetweenFlightsOnSchedule + SimulationController.Weather[FlightSchedule.weatherIndex]) * 60);

                    Debug.Log("Reordering: " + flightsList[i].GetFlightNumber() + " time: " + flightsList[i].GetTimeToCompare() + " difference: " + difference.TotalMinutes);

                    if ((int)difference.TotalSeconds < (SimulationController.TimeBetweenFlightsOnSchedule + SimulationController.Weather[FlightSchedule.weatherIndex]) * 60)
                    {
                        flag = false;
                        int differenceThreshold = (SimulationController.TimeBetweenFlightsOnSchedule + SimulationController.Weather[FlightSchedule.weatherIndex]) * 60 - (int)difference.TotalSeconds;
                        Debug.Log("pre: " + flightsList[i - 1].GetFlightNumber() + " curr: " + flightsList[i].GetFlightNumber() + " diffrence tresh " + differenceThreshold);
                        Debug.Log("pre: " + flightsList[i - 1].GetTimeToCompare() + " curr: " + flightsList[i].GetTimeToCompare() + " diffrence tresh " + difference.TotalMinutes);

                        int lastIndex = LastUsedIndexPerLane[Array.IndexOf(DifferentLaneUseTime, DifferentLaneUseTime.Min())];
                        int preiviousFlightGrade = flightsList[lastIndex].GetProblem().GetIssue() != null ? flightsList[lastIndex].GetProblem().GetIssue().GetGrade() : 0;
                        int currentFlightGrade = flightsList[i].GetProblem().GetIssue() != null ? flightsList[i].GetProblem().GetIssue().GetGrade() : 0;

                        if (currentFlightGrade > preiviousFlightGrade)
                        {
                            flightsList[lastIndex].ChangeEitherTime((int)difference.TotalSeconds + (SimulationController.TimeBetweenFlightsOnSchedule + SimulationController.Weather[FlightSchedule.weatherIndex]) * 60);
                            int preflightIndex = AirportManager.Instance.GetFlightSchedule().GetFlights().GetHeap().IndexOf(flightsList[lastIndex]);
                            AirportManager.Instance.GetFlightSchedule().GetFlights().HeapifyDown(preflightIndex);
                            index = lastIndex;
                        }

                        else
                        {
                            flightsList[i].ChangeEitherTime(differenceThreshold);
                            int currflightIndex = AirportManager.Instance.GetFlightSchedule().GetFlights().GetHeap().IndexOf(flightsList[i]);
                            AirportManager.Instance.GetFlightSchedule().GetFlights().HeapifyDown(currflightIndex);
                        }
                    }

                    int indexOfArray = Array.IndexOf(DifferentLaneUseTime, DifferentLaneUseTime.Min()); 
                    DifferentLaneUseTime[indexOfArray] = flightsList[index].GetTimeToCompare();
                    DifferentLaneUseTime[indexOfArray] = DifferentLaneUseTime[indexOfArray].AddSeconds(-DifferentLaneUseTime[indexOfArray].Second);
                    LastUsedIndexPerLane[indexOfArray] = i;
                    flightsList[index].SetRunway(indexOfArray + 1);
                }
            }

            AirportManager.Instance.newReorder();
        }

        /// <summary>
        /// Remove flight from the schedule 
        /// </summary>
        /// <param name="flight"></param>
        private static void RemoveFlightFromSchedule(Flight flight)
        {
            Debug.Log("Removing Flight from Schedule");

            AirportManager.Instance.GetFlightSchedule().GetFlights().RemoveNode(flight);
            AirportManager.Instance.AddFlightToRemovedFlights(flight);
        }

        /// <summary>
        /// Function that solves the Lightly Sick Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LightlySickSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Lightly Sick");
            Debug.Log("Solving Lightly Sick Issue");
        }

        /// <summary>
        /// Function that solves the Other Severe Sick Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void OtherSevereSickSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Other Severe Sick");
            Debug.Log("Solving Other Severe Sick Issue");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()]; 

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null 
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Lightly Wounded Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LightlyWoundedSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Lightly Wounded");
            Debug.Log("Solving Lightly Wounded Issue");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            ReorderFlightsSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Severely Wounded Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void SeverelyWoundedSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Severely Wounded"); 
            Debug.Log("Solving Severely Wounded Issue");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Dying Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void DyingSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Dying");
            Debug.Log("Solving Dying Issue");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                if (altenateAirport == null)
                {
                    Debug.Log("Passanger Died");
                    flight.SetStatus("Passanger Died");
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Not Enough Fuel Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void NotEnoughFuelSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Not Enough Fuel");
            Debug.Log("Solving Not Enough Fuel Issue");

            Airport altenateAirport = null;
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = flight.GetPlane().GetCurrentFuelLevel(currTime) * SimulationController.fuelPerKm;

            if (minRadius <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
            }

            if (minRadius > distanceToAirport || (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
            {
                ReorderFlightsSchedule(flight);
                return;
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
                flight.SetStatus("crashed:" + flight.GetStatus());
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
            }

            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Faster Burn Rate Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void FasterBurnRateSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Faster Burn Rate");
            Debug.Log("Solving FasterBurnRate...");

            Airport altenateAirport = null;
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = flight.GetPlane().GetMaxDistanceAvailable(currTime);

            if (minRadius <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
            }

            if (minRadius > distanceToAirport || (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
            {
                ReorderFlightsSchedule(flight);
                return;
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
                flight.SetStatus("crashed:" + flight.GetStatus());
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
            }

            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Broken Wheels Rate Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void BrokenWheelsSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Broken Wheels");
            Debug.Log("Solving Broken Wheels Issue...");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            ReorderFlightsSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Thrust Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void ThrustIssueSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Thrust Issue");
            Debug.Log("Solving Thrust Issue");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the  Loose Door  Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LooseDoorSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Loose Door");
            Debug.Log("Solving Loose Door Issue");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }


        private static void MissingDoorSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Missing Door");
            Debug.Log("Solving Missing Door Issue");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Brake Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void BrakeIssueSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Brake Issue");
            Debug.Log("Solving Brake Issue");

            int speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime);

            if (speed != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
                flight.SetEstimatedLandingTime(currTime.AddMinutes(distanceToAirport / speed));
            }

            ReorderFlightsSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Low Output Single Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LowOutputSingleSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Low Output Single");
            Debug.Log("Solving LowOutputSingle");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }


        /// <summary>
        /// Function that solves the Low Output Double Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LowOutputDoubleSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Low Output Double");
            Debug.Log("Solving LowOutputDouble");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = SimulationController.searchRadiusToLand[flight.GetProblem().GetIssue().GetGrade()];

            while (altenateAirport == null)
            {
                if (minDistance <= distanceToAirport)
                {
                    altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
                }

                if (minRadius > SimulationController.MaxRadiusDistance || minDistance > distanceToAirport || (altenateAirport != null
                    && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
                {
                    ReorderFlightsSchedule(flight);
                    return;
                }

                minRadius += SimulationController.RefineDistance;
            }

            flight.SetAlternateArrivalAirport(altenateAirport);
            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Engine Failure Single Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void EngineFailureSingleSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Single Failure");
            Debug.Log("Solving EngineFailureSingle");

            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = flight.GetPlane().GetMaxDistanceAvailable(currTime);

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
            }

            if (minRadius > distanceToAirport || (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
            {
                ReorderFlightsSchedule(flight);
                return;
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
                flight.SetStatus("crashed:" + flight.GetStatus());
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
            }

            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Function that solves the Engine Failure Double Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void EngineFailureDoubleSolver(Flight flight, DateTime currTime)
        {
            flight.SetStatus("Double Failure");
            Debug.Log("Solving EngineFailureDouble");

            Airport altenateAirport = null;

            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            int minRadius = flight.GetPlane().GetMaxDistanceAvailable(currTime);

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade(), minRadius);
            }

            if (minRadius > distanceToAirport || (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode())))
            {
                ReorderFlightsSchedule(flight);
                return;
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
                flight.SetStatus("crashed:" + flight.GetStatus());
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
            }

            RemoveFlightFromSchedule(flight);
        }

        /// <summary>
        /// Intialize the solver singleton
        /// </summary>
        /// <returns></returns>
        public static Solver InitializeSolver()
        {
            if (Instance == null)
                Instance = new Solver();
            return Instance;
        }
    }
}