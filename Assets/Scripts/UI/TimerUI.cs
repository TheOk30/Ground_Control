using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Assets.Scripts.Controler;

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
            accumulatedTime += Time.deltaTime * timeSpeedMultiplier; // Accumulate time with multiplier

            while (accumulatedTime >= 1.0f)
            {
                // Update timer display once per second
                UpdateTimerDisplay(); 
                accumulatedTime -= 1.0f;
            }

            yield return null;
        }
    }

    // Update the timer display
    void UpdateTimerDisplay()
    {
        time = time.AddSeconds(1); // Update time by 1 second
        timerText.text = time.ToString("HH:mm:ss") + "  " + time.ToString("d/M/y");
    }
}
