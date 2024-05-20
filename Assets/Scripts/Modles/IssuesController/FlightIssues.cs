using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    /// <summary>
    /// Class that holds the Issue Value
    /// </summary>
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

        /// <summary>
        /// Get the Id of the issue
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return id;
        }

        /// <summary>
        /// Get the code of the issue
        /// </summary>
        /// <returns></returns>
        public string GetCode()
        {
            return code;
        }

        /// <summary>
        /// Get the name of the issue
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Get the description of the issue
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Get the grade of the issue
        /// </summary>
        /// <returns></returns>
        public int GetGrade()
        {
            return grade;
        }
    }
}
