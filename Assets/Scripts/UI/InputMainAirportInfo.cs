using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

public class InputMainAirportInfo : MonoBehaviour
{
    [SerializeField] private TMP_InputField airportCodeInput;
    [SerializeField] private TMP_InputField numRunwayInput;
    [SerializeField] private TMP_InputField flightIntervalsInput;
    [SerializeField] private TMP_Text errorText;

    private void Start()
    {
        errorText.color = Color.red;
        errorText.text = "";
    }

    /// <summary>
    /// Function that gets called when the "ENTER"
    /// button has been clicked
    /// </summary>
    public void OnSubmit()
    {
        Regex airportCodePattern = new Regex(@"\b[A-Z]{3}\b");
        Regex runwayNumberPattern = new Regex(@"^[1-4]$");
        Regex flightIntervalPattern = new Regex(@"^([5-9]|[1-9]\d|[1-6]\d{2}|7[0-1][0-9]|720)$");

        Debug.Log("InSumbit");

        if (!ValidateInput(airportCodeInput.text, airportCodePattern))
        {
            errorText.text = "Airport code must be 3 capital characters long";
            Debug.Log("Airport code must be 3 characters long");

            return;
        }

        if (!ValidateInput(numRunwayInput.text, runwayNumberPattern))
        {
            errorText.text = "Number of runways must be an integer";
            Debug.Log("Number of runways must be an integer between  1- 5");

            return;
        }

        if (!ValidateInput(flightIntervalsInput.text, flightIntervalPattern))
        {
            errorText.text = "Flight intervals must be an integer";
            Debug.Log("Flight intervals must be an integer between 5 -720");
            return;
        }

        if(DataBaseManager.Instance.GetAllAirportInfoByCode(airportCodeInput.text) != null)
        {
            Debug.Log("Airport code found");
            Debug.Log("Airport code: " + airportCodeInput.text);
            Debug.Log("Number of runways: " + numRunwayInput.text);
            Debug.Log("Flight intervals: " + flightIntervalsInput.text);
            DataBaseManager.Instance.SetAirportInfo(airportCodeInput.text, int.Parse(numRunwayInput.text), int.Parse(flightIntervalsInput.text));
            LevelTransition.TransitionToScene("MainScene");
            return;
        }

        errorText.text = "Airport code not found";
        Debug.Log("Airport code not found");
    }   

    private bool ValidateInput(string input, Regex pattern)
    {
        return pattern.IsMatch(input);
    }
}
