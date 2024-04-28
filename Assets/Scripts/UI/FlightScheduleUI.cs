using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets;
using UnityEngine.EventSystems;
using static System.Net.Mime.MediaTypeNames;
using Unity.VisualScripting;
using Assets.Scripts.DataStructures;
using System;
using System.Xml;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class FlightScheduleUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;
    [SerializeField] private TMP_Text dealtWithText;
    [SerializeField] private TMP_Text removeText; 
    [SerializeField] private TMP_Text flightScheduleTEXT;

    private AirportManager am;
    private int numDealWithButtons = 0;
    private int numRemoveButtons = 0;
    private int numReorderButtons = 0;

    // Start is called before the first frame update
    void Start()
    {
        int sibilingIndex = 0;
        string airportcode = DataBaseManager.Instance.GetAirportCode();
        int numRunways = DataBaseManager.Instance.GetNumRunways();
        int flightIntervals = DataBaseManager.Instance.GetFlightInterval();

        Debug.Log("Airport Code: " + airportcode);
        Debug.Log("Number of Runways: " + numRunways);
        Debug.Log("Flight Intervals: " + flightIntervals);
        if (airportcode != "" && numRunways != 0 && flightIntervals != 0)
        {
            Debug.Log("entered");
            Airport a = DataBaseManager.Instance.GetAllAirportInfoByCode(airportcode);
            Debug.Log("Airport: " + a.GetAirportName());
            am = AirportManager.InitializeAirportManager(a, flightIntervals, numRunways);
            Debug.Log("Airport Manager flightIntervals: " + am.GetFlightIntervals());
            Debug.Log("Airport Manager numRunways: " + am.GetNumRunways());
            am.CreateFlightScheduleForAirport(0);

            flightScheduleTEXT.text = "Flight Schedule: " + am.GetFlightSchedule().GetFlights().GetSize();
            dealtWithText.text = "Flights Landed: " + numDealWithButtons;
            removeText.text = "Flights Removed: " + numRemoveButtons;

            flightScheduleTEXT.transform.SetSiblingIndex(sibilingIndex++);

            // Iterate through the sorted list
            foreach (Flight flight in am.GetFlightSchedule().GetFlights().GetSortedWithoutModifyingHeap())
            {
                createButton(flight, sibilingIndex++, "HH:mm");
            }

            dealtWithText.transform.SetSiblingIndex(sibilingIndex++);
            removeText.transform.SetSiblingIndex(sibilingIndex);
        }

        else
            LevelTransition.TransitionToScene("Login");

    }

    // Update is called once per frame
    void Update()
    {
        if (AirportManager.Instance != null)
            UpdateFlightScheduleUI();
    }

    private void UpdateFlightScheduleUI()
    {
        flightScheduleTEXT.text = "Flight Schedule: " + am.GetFlightSchedule().GetFlights().GetSize();

        // Check if there are new removed flights
        if (numRemoveButtons < am.GetRemovedFlights().Count)
        {
            removeText.text = "Flights Removed: " + ++numRemoveButtons;

            // Get the latest removed flight
            Flight latestRemovedFlight = am.GetRemovedFlights()[am.GetRemovedFlights().Count - 1];

            // Destroy the existing button with the same ID
            DestroyButtonByID(latestRemovedFlight.GetFlightNumber());

            ChangeLayoutGrid(removeText.name, latestRemovedFlight);
        }

        // Check if there are new landed flights
        if (numDealWithButtons < am.GetLandedFlights().Count)
        {
            dealtWithText.text = "Flights Landed: " + ++numDealWithButtons;

            // Get the latest landed flight
            Flight latestLandedFlight = am.GetLandedFlights()[am.GetLandedFlights().Count - 1];
            
            // Destroy the existing button with the same ID
            DestroyButtonByID(latestLandedFlight.GetFlightNumber());

            ChangeLayoutGrid(dealtWithText.name, latestLandedFlight);
        }

        // Check if there are any reordering of the flights
        if(numReorderButtons < am.GetNumReorders())
        {
            Debug.Log("Reordering");

            List<Flight> flights = am.GetFlightSchedule().GetFlights().GetSortedWithoutModifyingHeap();

            // Iterate through the sorted list
            foreach (Flight flight in flights)
            {
                DestroyButtonByID(flight.GetFlightNumber());
            }

            for (int i = 1; i < flights.Count; i++)
            {
                createButton(am.GetFlightSchedule().GetFlights().GetSortedWithoutModifyingHeap()[i-1], i, "HH:mm");
            }

            numReorderButtons++;
        }
        
        if (am.GetFlightSchedule().GetFlights().GetSize() == 0)
        {
            for (int i = 1; i < dealtWithText.transform.GetSiblingIndex(); i++)
            {
                DestroyButtonByID(buttonParent.transform.GetChild(i).GetComponent<FlightButtonExpansion>().GetID());
            }
        }
    }

    private void DestroyButtonByID(string buttonID)
    {
        // Iterate through the children of buttonParent
        foreach (Transform child in buttonParent.transform)
        {
            // Get the FlightButtonExpansion component of the button
            FlightButtonExpansion buttonExpansion = child.GetComponent<FlightButtonExpansion>();

            // Check if the FlightButtonExpansion component exists and its ID matches the desired value
            if (buttonExpansion != null && buttonExpansion.GetID().Equals(buttonID))
            {
                // Destroy the button game object
                Destroy(child.gameObject);
            }
        }
    }

    private void ChangeLayoutGrid(string childName, Flight flight)
    {
        int siblingIndex = 0;

        // Iterate through the children of buttonParent
        foreach (Transform child in buttonParent.transform)
        {
            child.SetSiblingIndex(siblingIndex++);

            if (child.name == childName)
            {
                createButton(flight, siblingIndex, "completed");
            }
        }
    }

    private void createButton(Flight flight, int index, string format)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
        newButton.GetComponent<FlightButtonExpansion>().flight_button_text.text = flight.ToString(format);
        newButton.GetComponent<FlightButtonExpansion>().Initialize(flight.GetFlightNumber());
        newButton.transform.SetSiblingIndex(index);
    }
}
