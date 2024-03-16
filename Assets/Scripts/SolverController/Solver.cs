using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static Assets.FlightIssues;

namespace Assets.Scripts.SolverController
{
    internal class Solver
    {
        private delegate void SolvingFunctions();

        // Define an array of functions for each enum value
        private static SolvingFunctions[] functionArray = new SolvingFunctions[]
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

        private static Dictionary<int, SolvingFunctions> SolvingFunctionsDict = PerformFunctionality();
        // Method to perform the functionality
        

        public Solver(Flight flight)
        {
            Debug.Log("Issue In solver" + flight.GetProblem().GetIssue().GetId());
            SolvingFunctionsDict[flight.GetProblem().GetIssue().GetId()].Invoke();
        }


        private static void LightlySickSolver()
        {
            Debug.Log("Solving LightlySick...");
        }

        private static void OtherSevereSickSolver()
        {
            Debug.Log("Solving OtherSevereSick...");
        }

        private static void LightlyWoundedSolver()
        {
            Debug.Log("Solving LightlyWounded...");
        }

        private static void SeverelyWoundedSolver()
        {
            Debug.Log("Solving SeverelyWounded...");
        }

        private static void DyingSolver()
        {
            Debug.Log("Solving Dying...");
        }

        private static void NotEnoughFuelSolver()
        {
            Debug.Log("Solving NotEnoughFuel...");
        }

        private static void FasterBurnRateSolver()
        {
            Debug.Log("Solving FasterBurnRate...");
        }

        private static void BrokenWheelsSolver()
        {
            Debug.Log("Solving BrokenWheels...");
        }

        private static void ThrustIssueSolver()
        {
            Debug.Log("Solving ThrustIssue...");
        }

        private static void LooseDoorSolver()
        {
            Debug.Log("Solving LooseDoor...");
        }

        private static void MissingDoorSolver()
        {
            Debug.Log("Solving MissingDoor...");
        }

        private static void BrakeIssueSolver()
        {
            Debug.Log("Solving BrakeIssue...");
        }

        private static void LowOutputSingleSolver()
        {
            Debug.Log("Solving LowOutputSingle...");
        }

        private static void LowOutputDoubleSolver()
        {
            Debug.Log("Solving LowOutputDouble...");
        }

        private static void EngineFailureSingleSolver()
        {
            Debug.Log("Solving EngineFailureSingle...");
        }

        private static void EngineFailureDoubleSolver()
        {
            Debug.Log("Solving EngineFailureDouble...");
        }

        private static Dictionary<int, SolvingFunctions> PerformFunctionality()
        {
            // Create a dictionary to hold enum values and their corresponding arrays of functions
            Dictionary<int, SolvingFunctions> SolvingFunctionsDict = new Dictionary<int, SolvingFunctions>();

            // Populate the dictionary with arrays of functions for each enum value
            for (int i = 0; i < functionArray.Length; i++)
            {
                SolvingFunctionsDict.Add(i + 1, functionArray[i]);
            }

            return SolvingFunctionsDict;
        }
    }
}
