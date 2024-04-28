using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Linq;
using System.Data;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Net;
using Unity.VisualScripting;
using Assets;
using UnityEngine.UIElements;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance;

    private string AirportCode = "";
    private int numRunways = 0;
    private int flightInterval = 0;

    /// <summary>
    /// Manage and remember variables in between scenes.
    /// If there are two instances of this script in a scene, 
    /// delete one of them
    /// This Instance is also the only way for outside classes to get access to the members of this Script
    /// since it is built as a true singleton, the instance of this class is only created once
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// set the airport info for the current airport
    /// </summary>
    /// <param name="airportCode"></param>
    /// <param name="numRunways"></param>
    /// <param name="flightInterval"></param>
    public void SetAirportInfo(string airportCode, int numRunways, int flightInterval)
    {
        this.AirportCode = airportCode;
        this.numRunways = numRunways;
        this.flightInterval = flightInterval;
    }

    public string GetAirportCode()
    {
        return AirportCode;
    }

    public int GetNumRunways()
    {
        return numRunways;
    }

    public int GetFlightInterval()
    {
        return flightInterval;
    }


    /// <summary>
    /// Inset the extracted airport info to the database
    /// Function used for populating the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="code"></param>
    /// <param name="lat"></param>
    /// <param name="lon"></param>
    /// <param name="country"></param>
    public void InsertExtractedAirportInfoToDB(int id, string name, string code, double lat, double lon, string country)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandInsertValues = dbConnection.CreateCommand();
        string idForDB = '"' + id.ToString() + '"' + ',';
        string nameForDB = '"' + name + '"' + ',';
        string codeForDB = '"' + code + '"' ;
        string latForDB = '"' + lat.ToString() + '"' + ',';
        string lonForDB = '"' + lon.ToString() + '"' + ',';
        string countryForDB = '"' + country + '"' + ',';

        dbCommandInsertValues.CommandText = "INSERT INTO AirportsTable (ID, Name, Latitude, Longitude, Country, AirportCode) VALUES(" + idForDB + nameForDB +  latForDB + lonForDB + countryForDB + codeForDB + ");";
        dbCommandInsertValues.ExecuteNonQuery();
        dbConnection.Close();
    }

    /// <summary>
    /// Insert to the Relationship Table the connection between them
    /// Created in mind for the usage of two tabels
    /// AirlinesToAirportsManager
    /// AirlinesToPlanesManager
    /// </summary>
    /// <param name="list"></param>
    /// <param name="id"></param>
    /// <param name="tableName"></param>
    /// <param name="otherElementName"></param>
    public void InsertItemWithConnectionToAirline(List<int> list, int id, string tableName, string mainElementName, string otherElementName)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandInsertValues = dbConnection.CreateCommand();
        string idForDB = '"' + id.ToString() + '"' + ',';
        string tableNameForDB = '"' + tableName + '"';
        string otherElementNameForDB = '"' + otherElementName + '"';
        string mainElementNameForDB = '"' + mainElementName + '"' + ',';
        string listItemForDB = "";
        list.Sort();

        for (int i = 0; i < list.Count; i++)
        {
            listItemForDB = '"' + list[i].ToString() + '"';
            dbCommandInsertValues.CommandText = "INSERT INTO" + tableNameForDB + "(" + mainElementNameForDB + otherElementNameForDB + ") VALUES(" + idForDB + listItemForDB + ");";
            dbCommandInsertValues.ExecuteNonQuery();
        }

        dbConnection.Close();
    }

    /// <summary>
    /// Get all the elements of a list matching the id
    /// Generic Function
    /// </summary>
    /// <param name="elementID"></param>
    /// <param name="itemSelected"></param>
    /// <param name="tableName"></param>
    /// <param name="conditionID"></param>
    /// <returns></returns>
    public List<int> GetListOfElemntsFromTableMatchingCondition(int elementID, string itemSelected, string tableName, string conditionID )
    {
        List<int> numlist = new List<int>();
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string elementIDForDB = '"' + elementID.ToString() + '"';
        string itemSelectedForDB = '"' + itemSelected + '"';
        string tableNameForDB = '"' + tableName + '"';
        string conditionIDForDB = '"' + conditionID + '"';

        dbCommandReadValues.CommandText = "SELECT" + itemSelectedForDB + "FROM" + tableNameForDB + "WHERE" + conditionIDForDB + "=" + elementIDForDB + ";";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        if (!(dataReader is null))
        {
            while (dataReader.Read())
            {
                numlist.Add(dataReader.GetInt32(0));
            }
        }

        dbConnection.Close();

        return numlist;
    }

    /// <summary>
    /// Add all the existing airports to a list as to not add them again and make duplicates.
    /// </summary>
    public void AddAirportCodeToList_InfoDb()
    {
        string AirportCode = "";
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT AirportCode FROM AirportsTable";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            AirportCode = dataReader.GetString(0);
            ExtractInformationToDB.AddAirportCodeToList(AirportCode);
        }

        dbConnection.Close();
    }

    /// <summary>
    /// Get the max Id from the table in order to add more rows
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public int GetTheLastIdFromTable(string tableName)
    {
        int ID = 0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string tableNameForDB = '"' + tableName.ToString() + '"';

        dbCommandReadValues.CommandText = "SELECT MAX(ID) FROM"+ tableNameForDB + ';';
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            ID = dataReader.GetInt32(0);
        }

        dbConnection.Close();

        return ID;
    }

    /// <summary>
    /// Get all the airlines's id that fly to the current airport
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<int> GetAirlinesFlyingToAirport(int id) 
    {
        List<int> airlines = new List<int>();
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string idForDB = '"' + id.ToString() + '"';

        dbCommandReadValues.CommandText = "SELECT AirlineID FROM AirlinesToAirportsManager WHERE AirportID =" + idForDB + ';';
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        
        while (dataReader.Read())
        {
            airlines.Add(dataReader.GetInt32(0));
        }

        dbConnection.Close();

        return airlines;
    }

    /// <summary>
    /// Get all the info for a all airport from the airports table
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<Airport> GetAllAirports()
    {
        List<Airport> airports = new List<Airport>();

        int id = 0;
        string name = "";
        double latitude = 0.0;
        double longitude = 0.0;
        string country = "";
        string city = "";
        string code = "";
        int runwayLength = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();

        dbCommandReadValues.CommandText = "SELECT ID, Name, Latitude, Longitude, Country, City, AirportCode, RunwayLength FROM AirportsTable ;";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            id = dataReader.GetInt32(0);
            name = dataReader.GetString(1);
            latitude = dataReader.GetDouble(2);
            longitude = dataReader.GetDouble(3);
            country = dataReader.GetString(4);

            try
            {
                if (dataReader.GetString(5) != null)
                    city = dataReader.GetString(5);
            }

            catch (InvalidCastException)
            {
                city = " ";
            }

            code = dataReader.GetString(6);

            try
            {
                if (dataReader.GetInt32(7) != null)
                    runwayLength = dataReader.GetInt32(7);

            }

            catch (InvalidCastException)
            {
                runwayLength = 2000;
            }

            airports.Add(new Airport(id, name, country, city, latitude, longitude, code, runwayLength));
        }

        dbConnection.Close();

        return airports;
    }


    /// <summary>
    /// Get all the info for a certain airport with a given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns> Returns an airport ObJect for the id provided</returns>
    public Airport GetAllAirportInfo(int id)
    {
        string name = "";
        double latitude = 0.0;
        double longitude = 0.0;
        string country = "";
        string city = "";
        string code = "";
        int runwayLength = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string idForDB = '"' + id.ToString() + '"';

        dbCommandReadValues.CommandText = "SELECT Name, Latitude, Longitude, Country, City, AirportCode, RunwayLength FROM AirportsTable WHERE ID =" + idForDB + ';';
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            name = dataReader.GetString(0);
            latitude = dataReader.GetDouble(1);
            longitude = dataReader.GetDouble(2);
            country = dataReader.GetString(3);

            try
            {
                if (dataReader.GetString(4) != null)
                    city = dataReader.GetString(4);
            }

            catch (InvalidCastException)
            {
                city = " ";
            }

            code = dataReader.GetString(5);

            try
            {
                if (dataReader.GetInt32(6) != null)
                    runwayLength = dataReader.GetInt32(6);
            }

            catch (InvalidCastException)
            {
                runwayLength = 2000;
            }
        }

        dbConnection.Close();

        return new Airport(id, name, country, city, latitude, longitude, code, runwayLength);
    }

    public Airport GetAllAirportInfoByCode(string code)
    {
        int id = 0;
        string name = "";
        double latitude = 0.0;
        double longitude = 0.0;
        string country = "";
        string city = "";
        int runwayLength = 0;

        using (IDbConnection dbConnection = CreateAndOpenDatabase())
        {
            using (IDbCommand dbCommandReadValues = dbConnection.CreateCommand())
            {
                dbCommandReadValues.CommandText = "SELECT ID, Name, Latitude, Longitude, Country, City, RunwayLength FROM AirportsTable WHERE AirportCode = @AirportCode;";
                IDbDataParameter codeParameter = dbCommandReadValues.CreateParameter();
                codeParameter.ParameterName = "@AirportCode";
                codeParameter.Value = code;
                dbCommandReadValues.Parameters.Add(codeParameter);

                using (IDataReader dataReader = dbCommandReadValues.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        id = dataReader.GetInt32(0);
                        name = dataReader.GetString(1);
                        latitude = dataReader.GetDouble(2);
                        longitude = dataReader.GetDouble(3);
                        country = dataReader.GetString(4);

                        // Check for null values before retrieving data
                        if (!dataReader.IsDBNull(5))
                        {
                            city = dataReader.GetString(5);
                        }

                        if (!dataReader.IsDBNull(6))
                        {
                            runwayLength = dataReader.GetInt32(6);
                        }
                    }
                    else
                    {
                        // Query returned no rows, handle it accordingly
                        return null; // Or any other appropriate action
                    }
                }
            }
        }

        return new Airport(id, name, country, city, latitude, longitude, code, runwayLength);
    }

    /// <summary>
    /// Get All the informarmation of the Airline for the id provided
    /// </summary>
    /// <param name="id"></param>
    /// <returns>An airline object containing all the necessery info for the airline</returns>
    public Airline GetAllAirlineInfo(int id)
    {
        string name = "";
        string AirlineCode = "";
        int HomeAirport = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string idForDB = '"' + id.ToString() + '"';

        dbCommandReadValues.CommandText = "SELECT Name, AirlineCode, HomeAirport FROM AirlineTable WHERE ID =" + idForDB + ';';
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            name = dataReader.GetString(0);
            AirlineCode = dataReader.GetString(1);
            HomeAirport = dataReader.GetInt32(2);
        }

        dbConnection.Close();

        return new Airline(id, name, AirlineCode, HomeAirport);
    }

    /// <summary>
    /// Select Random Airport From The Database for the Flight Schedule
    /// Used to find the other airport for creating the flight
    /// </summary>
    /// <param name="AirlineId"></param>
    /// <param name="MainAirportID"></param>
    /// <returns>id of the random airport</returns>
    public int SelectRandomAirportIdFromTable(int AirlineId, int MainAirportID)
    {
        int returnedId=0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string AirlineIdForDB = '"' + AirlineId.ToString() + '"';
        string MainAirportIDForDB = '"' + MainAirportID.ToString() + '"';

        dbCommandReadValues.CommandText = "SELECT AirportID FROM AirlinesToAirportsManager WHERE AirlineID =" + AirlineIdForDB + "AND AirportID <>" + MainAirportIDForDB + "ORDER BY RANDOM() LIMIT 1;";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        
        while (dataReader.Read())
        {
            returnedId = dataReader.GetInt32(0);
        }
   
        dbConnection.Close();
        return returnedId;
    }

    /// <summary>
    /// Get All the info of an Issue By the Id.
    /// Used to create the problem object an Issue has been found in the flight
    /// </summary>
    /// <param name="IssueId"></param>
    /// <returns>FlightIssues Object</returns>
    public FlightIssues GetIssueInfo(int IssueId)
    {
        string IssueCode = "";
        string IssueName = "";
        string Description = "";
        int Grade = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string IssueIdForDB = '"' + IssueId.ToString() + '"';

        string command = "SELECT IssueCode, IssueName, Description, Grade ";
        command += "FROM IssuesTable ";
        command += "WHERE ID = " + IssueIdForDB + " ;";
        dbCommandReadValues.CommandText = command;

        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            IssueCode = dataReader.GetString(0);
            IssueName = dataReader.GetString(1);
            Description = dataReader.GetString(2);
            Grade = dataReader.GetInt32(3);
        }

        dbConnection.Close();

        return new FlightIssues(IssueId, IssueCode, IssueName, Description, Grade);
    }

    /// <summary>
    /// Get the max range of the airline
    /// </summary>
    /// <param name="AirlineId"></param>
    /// <returns></returns>
    public int GetMaxFlightDistanceForAirline(int AirlineId)
    {
        int maxDistance = 0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string AirlineIdForDB = '"' + AirlineId.ToString() + '"';

        string command = "SELECT Max(MaxRange) ";
        command += "FROM PlanesTable Join AirlinesToPlanesManager ON AirlinesToPlanesManager.PlanesID = PlanesTable.ID ";
        command += "WHERE " + AirlineIdForDB + " = AirlinesToPlanesManager.AirlineID;";
        dbCommandReadValues.CommandText = command;

        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            maxDistance = dataReader.GetInt32(0);
        }

        dbConnection.Close();
        return maxDistance;
    }
   
    /// <summary>
    /// Creates a random plane for the flight based on the range of the flight and airline
    /// </summary>
    /// <param name="AirlineId"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public Assets.Plane GetRandomPlane(int AirlineId, int range)
    {
        string name = "";
        int fuelCapacity = 0;
        int FuelDropRate = 0;
        int avrSpeed = 0;
        int maxSpeed = 0;
        int maxRange = 0;
        int grade = 0;

        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string AirlineIdForDB = '"' + AirlineId.ToString() + '"';
        string rangeIDForDB = '"' + range.ToString() + '"';

        string command = "SELECT PlanesTable.Name, PlanesTable.FuelCapacity, PlanesTable.AvrSpeed, PlanesTable.FuelDropRate, PlanesTable.MaxRange, PlanesTable.MaxSpeed, PlanesTable.RunwayGrade ";
        command += "FROM PlanesTable Join AirlinesToPlanesManager ON AirlinesToPlanesManager.PlanesID = PlanesTable.ID ";
        command += "WHERE " + rangeIDForDB + " <= MaxRange AND " + AirlineIdForDB + " = AirlinesToPlanesManager.AirlineID ORDER BY RANDOM() LIMIT 1;";
        dbCommandReadValues.CommandText = command;

        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            name = dataReader.GetString(0);
            fuelCapacity = dataReader.GetInt32(1);
            avrSpeed = dataReader.GetInt32(2);
            FuelDropRate = dataReader.GetInt32(3);
            maxRange = dataReader.GetInt32(4);
            maxSpeed = dataReader.GetInt32(5);
            grade = dataReader.GetInt32(6);
        }

        dbConnection.Close();

        if (AirlineId == 0)
        {
            return null; 
        }

        return new Assets.Plane(AirlineId, name,fuelCapacity, FuelDropRate, avrSpeed, maxSpeed, maxRange, grade);
    }

    /// <summary>
    /// This function receives the table name, column name and a specific item in that column
    /// and returns the id of that row
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="item"></param>
    /// <param name="rowName"></param>
    /// <returns></returns>
    public int GetIdOfElement(string tableName, string item, string columnName)
    {
        int ID = 0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string tableNameForDB = '"' + tableName + '"';
        string columnNameForDB = '"' + columnName + '"';
        string itemForDB = '"' + item + '"';

        dbCommandReadValues.CommandText = "SELECT ID FROM" + tableNameForDB + "WHERE" + columnNameForDB + '='+ itemForDB + ';';
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            ID = dataReader.GetInt32(0);
        }

        dbConnection.Close();

        return ID;
    }

    /// <summary>
    /// Get the max value of coordinate either Longitude or Latitude
    /// </summary>
    /// <param name="coorName"></param>
    /// <returns></returns>
    public double GetMaxFromAirportTable(string coorName)
    {
        double value = 0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string coorNameForDB = '"' + coorName + '"';

        dbCommandReadValues.CommandText = "SELECT Max(" + coorNameForDB + ") FROM AirportsTable ;";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            value = dataReader.GetDouble(0);
        }

        dbConnection.Close();

        return value;
    }


    /// <summary>
    /// Get the min value of coordinate either Longitude or Latitude
    /// </summary>
    /// <param name="coorName"></param>
    /// <returns></returns>
    public double GetMinFromAirportTable(string coorName)
    {
        double value = 0;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string coorNameForDB = '"' + coorName + '"';

        dbCommandReadValues.CommandText = "SELECT Min(" + coorNameForDB + ") FROM AirportsTable ;";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while (dataReader.Read())
        {
            value = dataReader.GetDouble(0);
        }

        dbConnection.Close();

        return value;
    }

    /// <summary>
    /// This function is responsible for making the intial connection to the database
    /// And creating the new tabels in need to connect to if they do not exist at initial connection
    /// </summary>
    /// <returns></returns>
    private IDbConnection CreateAndOpenDatabase()
    {
        //Open a connection to the database
        string dbUri = "URI=file:GroundControlDatabase.sqlite";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        //Create a table for the Airport Table in the database if it does not exist yet
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS AirportsTable (ID INTEGER PRIMARY KEY, Name TEXT, Latitude REAL, Longitude REAL, Country TEXT, City TEXT, AirportCode TEXT)";
        dbCommandCreateTable.ExecuteReader();

        //Create a table for the Airline Table in the database if it does not exist yet
        dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS AirlineTable (ID INTEGER PRIMARY KEY, Name TEXT, AirlineCode TEXT, HomeAirport INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        //Create a table for the many to many relationship between Airline Table and Airport Table in the database if it does not exist yet
        dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS AirlinesToAirportsManager (AirlineID INTEGER, AirportID INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        //Create a table for the many to many relationship between Airline Table and Planes Table in the database if it does not exist yet
        dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS AirlinesToPlanesManager (AirlineID INTEGER, PlaneID INTEGER, Quantity INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        //Create a table for the Planes Table count in the database if it does not exist yet
        dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS PlanesTable (ID INTEGER PRIMARY KEY, Name TEXT, FuelCapacity INTEGER, AvrSpeed INTEGER, FuelDropRate INTEGER, MaxSpeed INTEGER, MaxRange INTEGER, RunwayGrade INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        //Create a table for the Planes Table count in the database if it does not exist yet
        dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS IssuesTable (ID INTEGER PRIMARY KEY, IssueCode TEXT, IssueName TEXT, Description TEXT, Grade INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        return dbConnection;
    }
}