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
        private int issueNumber;
        private int TimeTillApearenceInteger;
        private FlightIssues issue;
        private DateTime TimeTillApearence;
        private int HasProblem;

        public ProblemCreator(int FlightDuration, DateTime takeoffTime)
        {
            System.Random rnd = new System.Random();
            this.HasProblem = 0;
            this.issueNumber = rnd.Next(0, Enum.GetNames(typeof(FlightIssues.IssueCode)).Length); //Get the sizeof the enum. Safe for future enum changes
            this.TimeTillApearenceInteger = rnd.Next(SimulationController.First_LastProblemTimePossible, FlightDuration - SimulationController.First_LastProblemTimePossible);

            this.TimeTillApearence = takeoffTime.AddMinutes(TimeTillApearenceInteger);
            this.issue = null;
        }

        public FlightIssues GetIssue()
        {
            return issue;
        }

        public void MakeProblemAppear()
        {
            //Get all the problem info from the DataBase
            //inset the problem and assign the value to the 
            this.HasProblem = 1;
        }
    }
}
