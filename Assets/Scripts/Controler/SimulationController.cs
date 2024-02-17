using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controler
{
    /// <summary>
    /// Class that contains all paramates that are used throughout the system
    /// They will only be declared once so the program will be safe from using different parametes
    /// </summary>
    public static class SimulationController
    {
        //random(1,101) % 5 == 20% of a problem occuring
        public const int percentageOfProblem = 5;

        //The earliest and latest time where a porblem can occur on a flight
        public const int First_LastProblemTimePossible = 20;


        public const int TimeSpeeder = 10;
    }
}
