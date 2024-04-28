using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    // Method to transition to a new scene
    public static void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
