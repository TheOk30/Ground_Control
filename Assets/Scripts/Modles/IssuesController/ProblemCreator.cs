using Assets.Scripts.Controler;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Assets.FlightIssues;

namespace Assets.Scripts.Modles.IssuesControler
{
    public class ProblemCreator
    {
        private int issueNumber = 0;
        private int TimeTillApearenceInteger;
        private FlightIssues issue = null;
        private DateTime ApearenceTime;
        private int ActiveProblem = 0;

        public ProblemCreator(int FlightDuration, DateTime takeoffTime)
        {
            System.Random rnd = new System.Random();

            //flight chosen to have a problem 
            if (rnd.Next(1, 101) % SimulationController.percentageOfProblem == 0)
            {
                this.issueNumber = rnd.Next(0, Enum.GetNames(typeof(FlightIssues.IssueCode)).Length); //Get the sizeof the enum. Safe for future enum changes
                this.TimeTillApearenceInteger = rnd.Next(SimulationController.First_LastProblemTimePossible, FlightDuration - SimulationController.First_LastProblemTimePossible);
                this.ApearenceTime = takeoffTime.AddMinutes(TimeTillApearenceInteger);
            }           
        }

        public bool HasProblem(DateTime currentSystemTime)
        {
            if(this.ActiveProblem == 1)
                return true;

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
            //Get all the problem info from the DataBase
            //insert the problem and assign the value to the 
        }

        private bool CheckCountDown(DateTime currentSystemTime)
        {
            return this.ApearenceTime.Equals(currentSystemTime);
        }

        public FlightIssues GetIssue()
        {
            return issue;
        }  
    }
}
