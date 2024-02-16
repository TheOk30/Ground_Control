using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    class FlightIssues
    {
        private int id;
        private string name;
        private string description;
        private int grade;
        private string code;

        private static int numProblems =0;

        public FlightIssues()
        {
            this.id = numProblems++;
            this.name = " ";
            this.description = " ";
            this.grade = 0;
            this.code = "-1";
        }

        public FlightIssues(string name, string description, int grade, string code)
        {
            this.id = numProblems++;
            this.name = name;
            this.description = description;
            this.grade = grade;
            this.code = code;
        }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }
    }
}
