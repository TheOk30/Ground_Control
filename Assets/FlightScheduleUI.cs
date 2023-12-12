using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets;

public class FlightScheduleUI : MonoBehaviour
{
    public TextMeshProUGUI Text;

    // Start is called before the first frame update
    void Start()
    {
        Airport a = new Airport(1, "Ben Gurion Airport", "Israel", "Tel Aviv", 32.0011197883418, 34.8707382460235, "TLV", 3600);
        AirportManager am = AirportManager.InitializeAirportManager(a,60);
        am.CreateFlightScheduleForAirport(0);
        string str = am.GetFlightSchedule().ToString();
        Text.text = str;

    }

}
