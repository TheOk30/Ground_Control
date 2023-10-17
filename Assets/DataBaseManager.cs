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

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance;

    private void Start()
    {

    }

    /// <summary>
    /// Manage and remember variables in between scenes.
    /// If there are two instances of this script in a scene, 
    /// delete one of them
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
    /// Inset the extracted airport info to the database
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
    /// </summary>
    /// <param name="list"></param>
    /// <param name="id"></param>
    /// <param name="tableName"></param>
    /// <param name="otherElementID"></param>
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
    /// </summary>
    /// <param name="elementID"></param>
    /// <param name="itemSelected"></param>
    /// <param name="tableName"></param>
    /// <param name="conditionID"></param>
    /// <returns></returns>
    public List<int> GetListOfElemntsFromTableMatchingCondition(int elementID, string itemSelected, string tableName, string conditionID )
    {
        List<int> AirportFlysTo = new List<int>();
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        string elementIDForDB = '"' + elementID.ToString() + '"';
        string itemSelectedForDB = '"' + itemSelected + '"';
        string tableNameForDB = '"' + tableName + '"';
        string conditionIDForDB = '"' + conditionID + '"';


        dbCommandReadValues.CommandText = "SELECT" + itemSelectedForDB + "FROM" + tableNameForDB + "WHERE" + conditionIDForDB + "=" + elementIDForDB + ";";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        //dbCommandReadValues.CommandText = "SELECT AirportID FROM AirlinesToAirportsManager WHERE AirlineID =" + elementID + ";";

        if (!(dataReader is null))
        {
            while (dataReader.Read())
            {
                AirportFlysTo.Add(dataReader.GetInt32(0));
            }
        }

        dbConnection.Close();

        return AirportFlysTo;
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

        print(ID);
        return ID;
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
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS AirlineTable (ID INTEGER PRIMARY KEY, Name TEXT, AirlineCode TEXT, Planes TEXT, AirportsFlyingTo TEXT)";
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
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS PlanesTable (ID INTEGER PRIMARY KEY, Name TEXT, FuelCapacity INTEGER, AvrSpeed INTEGER, FuelDropRate INTEGER, MaxSpeed INTEGER)";
        dbCommandCreateTable.ExecuteReader();

        return dbConnection;
    }
}
