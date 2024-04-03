﻿using Assets.Scripts.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Assets.FlightIssues;
using static UnityEditor.FilePathAttribute;

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

            Debug.Log(Timer.time + " takeoff: " + flight.GetDepartingAirport() + " landing: " + flight.GetArrivalAirport() + " current location: " + flight.GetFlightLocation(Timer.time));
            Debug.Log((Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(Timer.time), flight.GetPlane().GetGrade()));
        }

        /// <summary>
        /// Reorder the flight schedule priority queue
        /// </summary>
        private static void ReorderFlightsSchedule()
        {

        }

        /// <summary>
        /// Remove flight from the schedule 
        /// </summary>
        /// <param name="flight"></param>
        private static void RemoveFlightFromSchedule(Flight flight)
        {

        }

        /// <summary>
        /// Function that solves the Lightly Sick Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LightlySickSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Lightly Sick Issue");
            ReorderFlightsSchedule();
        }

        /// <summary>
        /// Function that solves the Other Severe Sick Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void OtherSevereSickSolver(Flight flight, DateTime currTime)
        {
            int speed;
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Lightly Wounded Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LightlyWoundedSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Lightly Wounded Issue");
            int speed; 

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            ReorderFlightsSchedule();
        }

        /// <summary>
        /// Function that solves the Severely Wounded Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void SeverelyWoundedSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Severely Wounded Issue");
            int speed;
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Dying Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void DyingSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Dying Issue");
            int speed;
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Not Enough Fuel Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void NotEnoughFuelSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Not Enough Fuel Issue");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Faster Burn Rate Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void FasterBurnRateSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving FasterBurnRate...");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Broken Wheels Rate Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void BrokenWheelsSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Broken Wheels Issue...");
            int speed;

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            ReorderFlightsSchedule();
        }

        /// <summary>
        /// Function that solves the Thrust Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void ThrustIssueSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Thrust Issue");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the  Loose Door  Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LooseDoorSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Loose Door Issue");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }


        private static void MissingDoorSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Missing Door Issue");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Brake Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void BrakeIssueSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving Brake Issue");
            int speed;

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }

            ReorderFlightsSchedule();
        }

        /// <summary>
        /// Function that solves the Low Output Single Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LowOutputSingleSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving LowOutputSingle");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }


        /// <summary>
        /// Function that solves the Low Output Double Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void LowOutputDoubleSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving LowOutputDouble");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Engine Failure Single Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void EngineFailureSingleSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving EngineFailureSingle");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        /// <summary>
        /// Function that solves the Engine Failure Double Problem
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="currTime"></param>
        private static void EngineFailureDoubleSolver(Flight flight, DateTime currTime)
        {
            Debug.Log("Solving EngineFailureDouble");
            Airport altenateAirport = null;
            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());

            if (minDistance <= distanceToAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()))
            {
                ReorderFlightsSchedule();
            }

            else if (altenateAirport == null)
            {
                Debug.Log("No solution found -- KADISH");
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        public static Solver InitializeSolver()
        {
            if (Instance == null)
                Instance = new Solver();
            return Instance;
        }
    }
}