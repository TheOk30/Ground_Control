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
        //private int TimeTillApearenceInteger;
        private FlightIssues issue = null;
        private DateTime ApearenceTime;
        private int ActiveProblem = 0;

        public ProblemCreator(int FlightDuration, DateTime takeoffTime)
        {
            System.Random rnd = new System.Random();

            //flight chosen to have a problem 
            if (rnd.Next(1, 101) % SimulationController.percentageOfProblem == 0)
            {
                this.issueNumber = rnd.Next(1, DataBaseManager.Instance.GetTheLastIdFromTable("IssuesTable")+1); 
                int TimeTillApearenceInteger = rnd.Next(SimulationController.First_LastProblemTimePossible, FlightDuration - SimulationController.First_LastProblemTimePossible);
                this.ApearenceTime = takeoffTime.AddMinutes(TimeTillApearenceInteger);
                Debug.Log(this.issueNumber + " "+  this.ApearenceTime.ToString("HH:mm:ss"));
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
            if(this.ActiveProblem == 1)
                return false;

            if(this.issueNumber == 0)
                return false;

            if (CheckCountDown(currentSystemTime))
            {
                this.ActiveProblem = 1;
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
            return DateTime.Equals(this.ApearenceTime, currentSystemTime);
        }

        public FlightIssues GetIssue()
        {
            return this.issue;
        }  
    }
}
