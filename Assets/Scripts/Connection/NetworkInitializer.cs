using UnityEngine;

public class NetworkInitializer : MonoBehaviour
{
    public ApiClient apiClient;
    public ConnectionController connectionController;

    void Start()
    {
        var gm = GameModeManager.Instance;
        apiClient.baseUrl = gm.serverIP;
        connectionController.gameId = gm.gameId;

        switch (gm.currentMode)
        {
            case GameModeManager.GameMode.Singleplayer:
                apiClient.enabled = false;
                connectionController.enabled = false;
                Debug.Log("Modo SINGLEPLAYER iniciado.");
                break;

            /*
            case GameModeManager.GameMode.LAN_Host:
                apiClient.enabled = true;
                connectionController.enabled = true;
                gm.playerId = 0; // host siempre 0
                NetworkManagerLAN.Instance.InitializeLAN(gm.currentMode);
                Debug.Log($"Modo LAN HOST iniciado en {apiClient.baseUrl}");
                break;

            case GameModeManager.GameMode.LAN_Client:
                apiClient.enabled = true;
                connectionController.enabled = true;
                gm.playerId = 1; // cliente local
                NetworkManagerLAN.Instance.InitializeLAN(gm.currentMode);
                Debug.Log($"Modo LAN CLIENT conectado a {apiClient.baseUrl}");
                break;
            
            */
            case GameModeManager.GameMode.Dedicated_Client:
                apiClient.enabled = true;
                connectionController.enabled = true;
                gm.playerId = Random.Range(2, 9999); // ID aleatorio para evitar conflictos
                Debug.Log($"Conectando a servidor dedicado en {apiClient.baseUrl}");
                break;
        }
    }
}
