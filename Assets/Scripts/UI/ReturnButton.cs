using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
    public static void ReturnToMenu()
    {
        AirportManager.Instance = null;
        FlightSchedule.Instance = null;
        LevelTransition.TransitionToScene("Login");
    }
}
