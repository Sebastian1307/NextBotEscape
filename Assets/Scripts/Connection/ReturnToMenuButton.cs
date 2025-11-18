using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    public string menuSceneName = "MenuScreen"; 

    public void ReturnToMenu()
    {
        StartCoroutine(ReturnRoutine());
    }

    private System.Collections.IEnumerator ReturnRoutine()
    {
        // 1. Check for network runner
        var runner = FindObjectOfType<NetworkRunner>();

        if (runner != null)
        {
            // 2. Shutdown room
            yield return runner.Shutdown();
        }

        // 3. Load main menu scene, and loops back to game start
        SceneManager.LoadScene(menuSceneName);
    }
}
