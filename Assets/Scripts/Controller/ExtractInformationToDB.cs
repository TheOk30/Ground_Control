using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.Data;
using Assets;

/// <summary>
/// Class that Populates the database with airport info retrived from a website
/// </summary>
public class ExtractInformationToDB : MonoBehaviour
{
    private string infoAirport = "";
    [SerializeField] private bool flag = false;
    public static List<string> AirportCodes = new List<string>();

    public void Start()
    {
        if (flag)
        {
            DataBaseManager.Instance.AddAirportCodeToList_InfoDb();
        }
    }
    
    private void InsertInfo(int AirlineID)
    {
        string str;
        int Id = DataBaseManager.Instance.GetTheLastIdFromTable("AirportsTable");
        List<int> AirportFliesTo = DataBaseManager.Instance.GetListOfElemntsFromTableMatchingCondition(AirlineID, "AirportID", "AirlinesToAirportsManager", "AirlineID");
        List<int> AirportFliesToAdding = new List<int>();
        int AirportFlysToIDRecived = 0;
        string country = "";
        string code = "";
        string name = "";
        double lat = 0.0;
        double lon = 0.0;
        int i = 0;


        while (i != -1)
        {
            str = infoAirport.Substring(infoAirport.IndexOf("iata") + ("iata\": ").Length + 1);
            code = str.Substring(0, str.IndexOf("\""));

            if (!AirportCodes.Contains(code))
            {
                Id++;

                //infoAirport = infoAirport.Substring(infoAirport.IndexOf("country") + (country\": ").Length + 1);
                country = infoAirport.Substring(0, infoAirport.IndexOf("\""));

                infoAirport = infoAirport.Substring(infoAirport.IndexOf("lat") + ("lat\": ").Length);
                lat = double.Parse(infoAirport.Substring(0, infoAirport.IndexOf(",")), System.Globalization.CultureInfo.InvariantCulture);

                infoAirport = infoAirport.Substring(infoAirport.IndexOf("lon") + ("lon\": ").Length);
                lon = double.Parse(infoAirport.Substring(0, infoAirport.IndexOf(",")), System.Globalization.CultureInfo.InvariantCulture);

                infoAirport = infoAirport.Substring(infoAirport.IndexOf("name") + ("name\": ").Length + 1);
                name = infoAirport.Substring(0, infoAirport.IndexOf("\""));

                AddAirportCodeToList(code);

                DataBaseManager.Instance.InsertExtractedAirportInfoToDB(Id, name, code, lat, lon, country);
            }

            AirportFlysToIDRecived = DataBaseManager.Instance.GetIdOfElement("AirportsTable", code, "AirportCode");
            
            if (!AirportFliesTo.Contains(AirportFlysToIDRecived) && !AirportFliesToAdding.Contains(AirportFlysToIDRecived))
            {
                AirportFliesToAdding.Add(AirportFlysToIDRecived);
            }

            i = infoAirport.IndexOf("country");

            if (i != -1)
            {
                infoAirport = infoAirport.Substring(infoAirport.IndexOf("country") + ("country\": ").Length + 1);
            }
        }

        DataBaseManager.Instance.InsertItemWithConnectionToAirline(AirportFliesToAdding, AirlineID, "AirlinesToAirportsManager", "AirlineID", "AirportID");
    }

    public static void AddAirportCodeToList(string code)
    {
        if (!AirportCodes.Contains(code))
        {
            AirportCodes.Add(code);
        }
    }
}
