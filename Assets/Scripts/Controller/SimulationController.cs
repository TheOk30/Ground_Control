using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controller
{
    /// <summary>
    /// Class that contains all paramates that are used throughout the system
    /// They will only be declared once so the program will be safe from using different parametes
    /// </summary>
    public static class SimulationController
    {
        //random(1,101) % 5 == 20% of a problem occuring
        public const int percentageOfProblem = 2;

        //The earliest and latest time where a porblem can occur on a flight
        public const int First_LastProblemTimePossible = 20;

        //Time speeder for the simulation
        public const int TimeSpeeder = 3000;

        //Interval between flights for the flight schedule creator
        public const int TimeBetweenFlightsOnSchedule = 5;

        //Single engine minimum output can be reduced by 5%
        public const int SingleEngineRateMin = 5;

        //Single engine maximum output can be reduced by 45%
        public const int SingleEngineRateMax = 46;

        //Double engine minimum output can be reduced by 10%
        public const int DoubleEngineRateMin = 10;

        //Double engine maximum output can be reduced by 90%
        public const int DoubleEngineRateMax = 91;

        //Single Engine Failure Speed reduce flight speed to 80%
        public const int SingleEngineFailureSpeed = 80;

        //Double Engine Failure Speed reduce flight speed to 50%
        public const int DoubleEngineFailureSpeed = 50;

        //Fuel Burn Rate increase by 17%
        public const float fuelBurnRateDiffCruiseToMax = 1.17f;

        //The minimum Fuel percentage allowed when landing is 45 minutes of fuel so 75%
        //of the fuelConsumption per hour
        public const float minFuelPercentageAllowedAtLanding = 0.75f;

        //Random Object for the entire system
        public static System.Random rnd = new System.Random();

        //Dictionary for max radius from originial airport to start looking for an alternative landing location
        public static Dictionary<int, int> distanceToEmergencyLanding = new Dictionary<int, int>
        {
            { 2, -1 }, // Grade 2 - no need for landing
            { 4, -1 }, // Grade 4 - no need for landing
            { 5, -1 }, // Grade 5 - no need for landing
            { 6, 350 }, // Grade 6 - land in radius of 350
            { 7, 150 }, // Grade 7 - land in radius of 150
            { 8, 100 }, // Grade 8 - land in radius of 100
            { 9, 50 }, // Grade 9 - land in radius of 50
            { 10, 0 } // Grade 10 - land in radius of 0
        };

        //Dictionary for max radius from originial airport to start looking for an alternative landing location
        public static Dictionary<int, int> searchRadiusToLand = new Dictionary<int, int>
        {
            { 2, -1 }, // Grade 2 - no need for landing
            { 4, -1 }, // Grade 4 - no need for landing
            { 5, -1 }, // Grade 5 - no need for landing
            { 6, 300 }, // Grade 6 - land in radius of 350
            { 7, 350 }, // Grade 7 - land in radius of 150
            { 8, 400 }, // Grade 8 - land in radius of 100
            { 9, 500 }, // Grade 9 - land in radius of 50
            { 10, 550 } // Grade 10 - land in radius of 0
        };

        //Expand the search for an alternative landing location
        public static int RefineDistance = 30;

        //kg fuel consumption per km
        public static int fuelPerKm = 3;

        //Max distance to look for an alternative landing location
        public static int MaxRadiusDistance = 1000;

        //Dictionary for max radius from originial airport to start looking for an alternative landing location
        public static Dictionary<int, int> Weather = new Dictionary<int, int>
        {
            { 0, 0 }, // none
            { 1, 1 }, // rain
            { 2, 1 }, // snow
            { 3, 2 } // fog
        };

        public static bool chanceOfWeather = (rnd.Next(0, 100) % 7) == 0;
    }
}
