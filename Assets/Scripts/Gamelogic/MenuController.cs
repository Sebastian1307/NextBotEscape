using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Campos UI")]
    public TMP_InputField ipInputLAN;
    public TMP_InputField ipInputDedicated;

    // --- SINGLEPLAYER ---
    public void PlaySingleplayer()
    {
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.Singleplayer);
        SceneManager.LoadScene("Game");
    }

    // --- LAN ---
    public void PlayLAN_Host()
    {
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.LAN_Host);
        SceneManager.LoadScene("Game");
    }

    public void PlayLAN_Client()
    {
        string ip = ipInputLAN != null ? ipInputLAN.text : "http://localhost:5005/server";
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.LAN_Client, ip);
        SceneManager.LoadScene("Game");
    }

    // --- SERVIDOR DEDICADO ---
    public void PlayDedicatedClient()
    {
        string ip = ipInputDedicated != null ? ipInputDedicated.text : "http://mi-servidor.com/server";
        GameModeManager.Instance.SetMode(GameModeManager.GameMode.Dedicated_Client, ip);
        SceneManager.LoadScene("Game");
    }
}
