using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class PlaneQuantityManager
    {
        private int airlineID;
        private int planeID;
        private int quantity;
        private int planesCreated;

        public PlaneQuantityManager(int airlineID, int planeID, int quantity)
        {
            this.airlineID = airlineID;
            this.planeID = planeID;
            this.quantity = quantity;
            this.planesCreated = 0;
        }

        public int GetAirlineID() 
        {  
            return this.airlineID; 
        }

        public int GetPlaneID() 
        {  
            return this.planeID; 
        }

        public int GetQuantity()
        {
            return this.quantity; 
        }

        public int GetPlanesCreated()
        {
            return planesCreated;
        }
    }
}
