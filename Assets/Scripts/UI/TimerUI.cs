using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Assets.Scripts.Controller;
using Assets;
using Assets.Scripts.Modles.IssuesControler;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using Assets.Scripts.SolverController;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private bool isRunning = true;
    private float timeSpeedMultiplier = SimulationController.TimeSpeeder; // Make time passage 10 times faster
    public static DateTime time;
    private Solver solver;
    private float accumulatedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "";
        DateTime today = DateTime.Today;
        time = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
        StartCoroutine(UpdateTimeCoroutine());
        solver = Solver.InitializeSolver();
    }

    // Coroutine to update time at fixed intervals
    IEnumerator UpdateTimeCoroutine()
    {
        while (isRunning)
        {
            // Accumulate time with multiplier
            accumulatedTime += Time.deltaTime * this.timeSpeedMultiplier; 

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

        // The time is midnight (00:00:00)
        if (time.TimeOfDay == TimeSpan.Zero)
        {
            ChangeFlightScheduleActivator(time);
        }
    }

    private void CheckIfFlightsHaveIssues(DateTime time)
    {
        if (AirportManager.Instance == null)
            return;

        List<Flight> flightsList = AirportManager.Instance.GetFlightSchedule().GetFlights().GetSortedWithoutModifyingHeap();
        foreach (Flight flight in flightsList)
        { 
            if (flight.DidFlightTakeoff() && flight.GetProblem().HasProblem(time))
            {
                print(flight.ToString());
                print(flight.GetProblem().GetIssue().GetName().ToString());
                print(time.ToString("HH:mm:ss") + "  " + time.ToString("d/M/y"));

                if (ProblemCreator.problemFunctionsDict.ContainsKey(flight.GetProblem().GetIssue().GetId()))
                {
                    ProblemCreator.problemFunctionsDict[flight.GetProblem().GetIssue().GetId()].Invoke(flight);
                }

                solver.SolveFlightIssue(flight);
            }

            if(!flight.DidFlightTakeoff() && flight.GetTakeOffTime() ==  time) 
            {
                flight.FlightTookOff();
            }
        }
    }
     
    private void ChangeFlightScheduleActivator(DateTime time)
    {
       //
    }
}
