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
        // While there are still flights in the schedule
        // that haven't been handled yet, and the number
        // of daily flights is greater than 0
        while (isRunning)
        {
            // Check if AirportManager.Instance exists and flight schedule is available
            if (AirportManager.Instance != null && AirportManager.Instance.GetFlightSchedule() != null)
            {
                // Check if the number of daily flights is greater than 0
                if (AirportManager.Instance.GetFlightSchedule().GetNumberOfDailyFlights() > 0)
                {
                    // Accumulate time with multiplier
                    accumulatedTime += Time.deltaTime * this.timeSpeedMultiplier;

                    while (accumulatedTime >= 1.0f)
                    {
                        // Update timer display once per second
                        UpdateTimerDisplay();
                        accumulatedTime -= 1.0f;
                    }
                }

                else
                {
                    // If there are no flights, stop the simulation
                    isRunning = false;
                }
            }

            // Return control back to Unity after the asynchronous function
            yield return null;
        }

        Debug.Log("All Flights have been handled - Simulation Over");
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

            if (flight.DidFlightTakeoff() && !flight.FlightHasLanded() && flight.GetLandingTime() == time)
            {
                flight.FlightLanded();
                Debug.Log("Flight Landed");
                AirportManager.Instance.GetFlightSchedule().GetFlights().RemoveNode(flight);
                AirportManager.Instance.AddFlightToLandedFlights(flight);
            }
        }
    }
}
