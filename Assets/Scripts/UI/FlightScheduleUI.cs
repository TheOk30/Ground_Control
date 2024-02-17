using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets;
using static System.Net.Mime.MediaTypeNames;
using Unity.VisualScripting;

public class FlightScheduleUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;

    // Start is called before the first frame update
    private void Start()
    {
        Airport a = new Airport(1, "Ben Gurion Airport", "Israel", "Tel Aviv", 32.0011197883418, 34.8707382460235, "TLV", 3600);
        AirportManager am = AirportManager.InitializeAirportManager(a, 60,1);
        am.CreateFlightScheduleForAirport(0);

        foreach (Flight flight in am.GetFlightSchedule().GetFlights())
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            newButton.GetComponent<FlightButtonExpansion>().flight_button_text.text = flight.ToString("HH:mm");
        }      
    }

    private void SelectLevel()
    {
        Debug.Log("Loaded");  
    }
}
