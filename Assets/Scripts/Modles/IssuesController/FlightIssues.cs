using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class FlightIssues
    {
        private int id;
        private string name;
        private string description;
        private int grade;
        private string code;


        public FlightIssues()
        {
            this.id = 0;
            this.name = " ";
            this.description = " ";
            this.grade = 0;
            this.code = "-1";
        }

        public FlightIssues(int id, string code, string name, string description, int grade)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.description = description;
            this.grade = grade;
        }

        public int GetId()
        {
            return id;
        }

        public string GetCode()
        {
            return code;
        }

        public string GetName()
        {
            return name;
        }

        //public enum Issues
        //{
        //    LightlySick = 1,
        //    OtherSevereSick,
        //    LightlyWounded,
        //    SeverelyWounded,
        //    Dying,
        //    NotEnoughFuel,
        //    FasterBurnRate,
        //    BrokenWheels,
        //    ThrustIssue,
        //    LooseDoor,
        //    MissingDoor,
        //    BrakeIssue,
        //    LowOutputSingle,
        //    LowOutputDouble,
        //    EngineFailueSingle,
        //    EngineFailueDouble
        //}
    }
}
