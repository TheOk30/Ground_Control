using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Assets.Scripts.Controler;
using Assets;
using Assets.Scripts.Modles.IssuesControler;
using Unity.VisualScripting;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private bool isRunning = true;
    [SerializeField] private float timeSpeedMultiplier = (float)(SimulationController.TimeSpeeder); // Make time passage 10 times faster
    public DateTime time;

    private float accumulatedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "";
        DateTime today = DateTime.Today;
        time = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
        StartCoroutine(UpdateTimeCoroutine());
    }

    // Coroutine to update time at fixed intervals
    IEnumerator UpdateTimeCoroutine()
    {
        while (isRunning)
        {
            // Accumulate time with multiplier
            accumulatedTime += Time.deltaTime * timeSpeedMultiplier; 

            while (accumulatedTime >= 1.0f)
            {
                // Update timer display once per second
                UpdateTimerDisplay(); 
                accumulatedTime -= 1.0f;
            }

            //returns the controls back to unity after the asynchronous function
            yield return null;
        }
    }

    // Update the timer display
    void UpdateTimerDisplay()
    {
        // Update time by 1 second
        time = time.AddSeconds(1);

        // Desplay time in the format of 03:12:06  17/2/24
        timerText.text = time.ToString("HH:mm:ss") + "  " + time.ToString("d/M/y");

        if (time.Second == 0)
        {
            CheckIfFlightsHaveIssues(time);
        }
    }


    private void CheckIfFlightsHaveIssues(DateTime time)
    {
        if (AirportManager.Instance == null)
            return;
        
        foreach (Flight flight in AirportManager.Instance.GetFlightSchedule().GetFlights())
        {
            if (flight.GetProblem().HasProblem(time))
            {
                // Do something probably start executing the solver 
            }
        }

    }




    
}
