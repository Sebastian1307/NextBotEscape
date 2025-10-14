using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode { Singleplayer, LAN_Host, LAN_Client, Dedicated_Client }
    public GameMode currentMode = GameMode.Singleplayer;

    public string serverIP = "http://localhost:5005/server";
    public string gameId = "777";

    public int playerId = 0; // se asigna según el rol

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMode(GameMode mode, string ip = "")
    {
        currentMode = mode;
        if (!string.IsNullOrEmpty(ip))
            serverIP = ip;
    }
}
