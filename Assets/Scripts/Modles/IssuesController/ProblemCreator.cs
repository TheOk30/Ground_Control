using Assets.Scripts.Controller;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Modles.IssuesControler
{
    public class ProblemCreator
    {
        private int issueNumber = 0;
        private FlightIssues issue = null;
        private DateTime apearenceTime;
        private int activeProblem = 0;
        private static int count = 0;

        public delegate void ProblemFunctions(Flight flight);

        public static ProblemFunctions[] functionArray = new ProblemFunctions[]
        {
            NotEnoughFuelProblem,
            FasterBurnRateProblem,
            ThrustProblem,
            LowOutputSingleProblem,
            LowOutputDoubleProblem,
            EngineFailureSingleProblem,
            EngineFailureDoubleProblem
        };

        public static Dictionary<int, ProblemFunctions> problemFunctionsDict;


        public ProblemCreator(int FlightDuration, DateTime takeoffTime)
        {
            if(count == 0)
            {
                PerformFunctionality();
                count++;
            }

            //flight chosen to have a problem 
            if (SimulationController.rnd.Next(1, 101) % SimulationController.percentageOfProblem == 0)
            {
                this.issueNumber = SimulationController.rnd.Next(1, DataBaseManager.Instance.GetTheLastIdFromTable("IssuesTable")+1); 
                int TimeTillApearenceInteger = SimulationController.rnd.Next(SimulationController.First_LastProblemTimePossible, FlightDuration - SimulationController.First_LastProblemTimePossible);
                this.apearenceTime = takeoffTime.AddMinutes(TimeTillApearenceInteger);
                Debug.Log(this.issueNumber + " "+  this.apearenceTime.ToString("HH:mm:ss"));
            }           
        }

        /// <summary>
        /// Function checks if the problem has accured 
        /// Acts as an interface to check problems for every flight Interface 
        /// </summary>
        /// <param name="currentSystemTime"></param>
        /// <returns>
        ///     true if the problem has been activated
        ///     false if there is there is no problem active
        /// </returns>
        public bool HasProblem(DateTime currentSystemTime)
        {
            if(this.activeProblem == 1)
                return false;

            if(this.issueNumber == 0)
                return false;

            if (CheckCountDown(currentSystemTime))
            {
                this.activeProblem = 1;
                MakeProblemAppear();
                return true;
            }

            return false;
        }

        private void MakeProblemAppear()
        {
            this.issue = DataBaseManager.Instance.GetIssueInfo(this.issueNumber); 
        }

        private bool CheckCountDown(DateTime currentSystemTime)
        {
            return DateTime.Equals(this.apearenceTime, currentSystemTime);
        }

        public FlightIssues GetIssue()
        {
            return this.issue;
        }

        public static void PerformFunctionality()
        {
            // Create a dictionary to hold enum values and their corresponding arrays of functions
            problemFunctionsDict = new Dictionary<int, ProblemFunctions>
            {
                // Populate the dictionary with arrays of functions for each enum value
                { 6, functionArray[0] },
                { 7, functionArray[1] },
                { 9, functionArray[2] },
                { 13, functionArray[3] },
                { 14, functionArray[4] },
                { 15, functionArray[5] },
                { 16, functionArray[6] }
            };
        }

        private static void NotEnoughFuelProblem(Flight flight)
        {
            int newFuelLevel = (SimulationController.rnd.Next(SimulationController.DoubleEngineRateMin, SimulationController.DoubleEngineRateMax) * flight.GetPlane().GetCurrentFuelLevel(Timer.time)) / 100;
            flight.GetPlane().SetNewCurrentFuelLevel(newFuelLevel);
        }

        private static void FasterBurnRateProblem(Flight flight)
        {
            int newFuelLevel = (SimulationController.rnd.Next(SimulationController.DoubleEngineRateMin + 100, SimulationController.DoubleEngineRateMax + 100) * flight.GetPlane().GetFuelDropRate()) / 100;
            flight.GetPlane().SetNewFuelDropRate(newFuelLevel);
        }

        private static void ThrustProblem(Flight flight)
        {
            int newSpeed = (SimulationController.rnd.Next(SimulationController.DoubleEngineRateMin, SimulationController.DoubleEngineRateMax) * flight.GetPlane().GetCurrentSpeed()) / 100;
            flight.GetPlane().SetCurrentSpeed(newSpeed);
        }

        private static void LowOutputSingleProblem(Flight flight)
        {
            int newSpeed = (SimulationController.rnd.Next(SimulationController.SingleEngineRateMin, SimulationController.SingleEngineRateMax) * flight.GetPlane().GetCurrentSpeed()) / 100;
            flight.GetPlane().SetCurrentSpeed(newSpeed);
        }

        private static void LowOutputDoubleProblem(Flight flight)
        {
            int newSpeed = (SimulationController.rnd.Next(SimulationController.DoubleEngineRateMin, SimulationController.DoubleEngineRateMax) * flight.GetPlane().GetCurrentSpeed()) / 100;
            flight.GetPlane().SetCurrentSpeed(newSpeed);
        }

        private static void EngineFailureSingleProblem(Flight flight)
        {
            int newSpeed = (SimulationController.SingleEngineFailureSpeed * flight.GetPlane().GetCurrentSpeed()) / 100;
            flight.GetPlane().SetCurrentSpeed(newSpeed);
        }

        private static void EngineFailureDoubleProblem(Flight flight)
        {
            int newSpeed = (SimulationController.DoubleEngineFailureSpeed * flight.GetPlane().GetCurrentSpeed()) / 100;
            flight.GetPlane().SetCurrentSpeed(newSpeed);
        }
    }
}