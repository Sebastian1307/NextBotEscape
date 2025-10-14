
using System.Collections;
using UnityEngine;

public class PositionSync : MonoBehaviour
{
    [Header("Configuración")]
    public ApiClient apiClient;
    public string gameId = "terror";
    public int localPlayerId = 0;
    public int targetPlayerId = 1; // el otro jugador o enemigo

    [Header("Ajustes de sincronización")]
    public float updateInterval = 0.1f;
    public bool isLocalPlayer = true;

    private Vector3 targetPosition;

    private void Start()
    {
        if (apiClient == null)
        {
            Debug.LogError("Falta asignar ApiClient en PositionSync.");
            enabled = false;
            return;
        }

        apiClient.OnDataReceived += OnDataReceived;
        StartCoroutine(SyncLoop());
    }

    private IEnumerator SyncLoop()
    {
        while (true)
        {
            if (isLocalPlayer)
            {
                // Enviar pos servidor
                ServerData data = new ServerData
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z
                };
                StartCoroutine(apiClient.PostPlayerData(gameId, localPlayerId.ToString(), data));
            }
            else
            {
                // Recibir pos servidor
                StartCoroutine(apiClient.GetPlayerData(gameId, targetPlayerId.ToString()));
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            // Interpolacion mov
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        }
    }

    private void OnDataReceived(int playerId, ServerData data)
    {
        if (playerId == targetPlayerId)
        {
            targetPosition = new Vector3(data.posX, data.posY, data.posZ);
        }
    }
}