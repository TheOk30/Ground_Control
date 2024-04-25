using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlightButtonExpansion : MonoBehaviour
{
    public TextMeshProUGUI flight_button_text;

    // Public property to hold the ID of the flight
    private string flightID;

    // Method to initialize the flight ID
    public void Initialize(string id)
    {
        this.flightID = id;
    }

    public string GetID()
    {
        return this.flightID;
    }
}
