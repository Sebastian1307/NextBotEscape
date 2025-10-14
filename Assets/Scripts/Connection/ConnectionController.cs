
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [SerializeField] private ApiClient api;
    [SerializeField] private List<PlayerData> players;
    public string gameId;

    private float updateInterval = 0.1f;

    private void Start()
    {
        api.OnDataReceived += OnDataReceived;
        StartCoroutine(SyncPlayers());
    }

    private IEnumerator SyncPlayers()
    {
        while (true)
        {
            for (int i = 0; i < players.Count; i++)
            {
                //pos jugador servidor
                SendPlayerPosition(i);

                // Pedir pos al sv otros jugadores 
                StartCoroutine(api.GetPlayerData(gameId, i.ToString()));
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    public void OnDataReceived(int playerId, ServerData data)
    {
        if (playerId < 0 || playerId >= players.Count) return;

        Vector3 serverPos = new Vector3(data.posX, data.posY, data.posZ);

        // sincronizar pos jugador
        players[playerId].MovePlayer(serverPos);
    }

    public void SendPlayerPosition(int playerId)
    {
        if (playerId < 0 || playerId >= players.Count) return;

        Vector3 position = players[playerId].GetPosition();
        ServerData data = new ServerData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z
        };

        StartCoroutine(api.PostPlayerData(gameId, playerId.ToString(), data));
    }
}