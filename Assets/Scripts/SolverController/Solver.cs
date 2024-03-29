using Assets.Scripts.Controller;
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
        private delegate void SolvingFunctions(Flight flight);

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

        public void SolveFlightIssue(Flight flight)
        {
            //Debug.Log("Issue In solver" + flight.GetProblem().GetIssue().GetId());
            solvingFunctionsDict[flight.GetProblem().GetIssue().GetId()].Invoke(flight);    
        }

        private static void ReorderFlightsSchedule()
        {

        }

        private static void RemoveFlightFromSchedule(Flight flight)
        {

        }

        private static void LightlySickSolver(Flight flight)
        {
            ReorderFlightsSchedule();
        }


        private static void OtherSevereSickSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
            int speed;
            Airport altenateAirport = null;

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }


            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToOriginalAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
            if (minDistance <= distanceToOriginalAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            int distanceToAlternateAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), altenateAirport);

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()) && distanceToAlternateAirport >= distanceToOriginalAirport)
            {
                ReorderFlightsSchedule();
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        private static void LightlyWoundedSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
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

        private static void SeverelyWoundedSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
            int speed;
            Airport altenateAirport = null;

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }


            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToOriginalAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
            if (minDistance <= distanceToOriginalAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            int distanceToAlternateAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), altenateAirport);

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()) && distanceToAlternateAirport >= distanceToOriginalAirport)
            {
                ReorderFlightsSchedule();
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        private static void DyingSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
            int speed;
            Airport altenateAirport = null;

            if ((speed = flight.GetPlane().CalculateMaxSpeedIncrease(currTime)) != -1)
            {
                flight.GetPlane().SetCurrentSpeed(speed);
                int newFuelDropRate = (int)(flight.GetPlane().GetFuelDropRate() * SimulationController.fuelBurnRateDiffCruiseToMax);
                flight.GetPlane().SetNewFuelDropRate(newFuelDropRate);

                flight.SetEstimatedLandingTime(currTime.AddMinutes(flight.GetTimeTraveledMin(currTime)));
            }


            int minDistance = SimulationController.distanceToEmergencyLanding[flight.GetProblem().GetIssue().GetGrade()];
            int distanceToOriginalAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), flight.GetArrivalAirport());
            if (minDistance <= distanceToOriginalAirport)
            {
                altenateAirport = (Airport)spatialHashLocation.FindClosestLocations(flight.GetFlightLocation(currTime), flight.GetPlane().GetGrade());
            }

            int distanceToAlternateAirport = DistanceAndLocationsFunctions.DistanceBetweenCoordinates(flight.GetFlightLocation(currTime), altenateAirport);

            if (altenateAirport != null && altenateAirport.GetAirportCode().Equals(flight.GetArrivalAirport().GetAirportCode()) && distanceToAlternateAirport >= distanceToOriginalAirport)
            {
                ReorderFlightsSchedule();
            }

            else
            {
                flight.SetAlternateArrivalAirport(altenateAirport);
                RemoveFlightFromSchedule(flight);
            }
        }

        private static void NotEnoughFuelSolver(Flight flight)
        {
            Debug.Log("Solving NotEnoughFuel...");
        }

        private static void FasterBurnRateSolver(Flight flight)
        {
            Debug.Log("Solving FasterBurnRate...");
        }

        private static void BrokenWheelsSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
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

        private static void ThrustIssueSolver(Flight flight)
        {
            Debug.Log("Solving ThrustIssue...");
        }

        private static void LooseDoorSolver(Flight flight)
        {
            Debug.Log("Solving LooseDoor...");
        }

        private static void MissingDoorSolver(Flight flight)
        {
            Debug.Log("Solving MissingDoor...");
        }

        private static void BrakeIssueSolver(Flight flight)
        {
            DateTime currTime = Timer.time;
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

        private static void LowOutputSingleSolver(Flight flight)
        {
            Debug.Log("Solving LowOutputSingle...");
        }

        private static void LowOutputDoubleSolver(Flight flight)
        {
            Debug.Log("Solving LowOutputDouble...");
        }

        private static void EngineFailureSingleSolver(Flight flight)
        {
            Debug.Log("Solving EngineFailureSingle...");
        }

        private static void EngineFailureDoubleSolver(Flight flight)
        {
            Debug.Log("Solving EngineFailureDouble...");
        }

        public static Solver InitializeSolver()
        {
            if (Instance == null)
                Instance = new Solver();
            return Instance;
        }
    }
}