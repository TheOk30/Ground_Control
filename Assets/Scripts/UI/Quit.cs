using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    // This method needs to be public to be accessible from UI buttons or other scripts
    public void QuitGame()
    {
        Application.Quit();
    }
}