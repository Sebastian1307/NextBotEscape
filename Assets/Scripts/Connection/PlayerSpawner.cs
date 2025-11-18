using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkObject playerPrefab;
    public Transform[] spawnPoints;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined: " + player);

        if (!runner.IsServer)
        {
            Debug.Log("Client cant spawn players.");
            return;
        }

        int index = player.RawEncoded % spawnPoints.Length;

        NetworkObject obj = runner.Spawn(
            playerPrefab,
            spawnPoints[index].position,
            spawnPoints[index].rotation,
            player
        );

        runner.SetPlayerObject(player, obj);

        Debug.Log("Player spawned and synced: " + obj);
    }



    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        NetworkObject playerObj = runner.GetPlayerObject(player);

        if (playerObj != null)
        {
            runner.Despawn(playerObj);
        }
    }




    // -------- INPUT --------
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerInputData data = new PlayerInputData();

        data.x = Input.GetAxisRaw("Horizontal");
        data.z = Input.GetAxisRaw("Vertical");

        data.mouseX = Input.GetAxisRaw("Mouse X");
        data.mouseY = Input.GetAxisRaw("Mouse Y");

        data.jump = Input.GetButton("Jump");
        data.crouch = Input.GetKey(KeyCode.LeftControl);
        data.run = Input.GetKey(KeyCode.LeftShift);
        data.flashlight = Input.GetKeyDown(KeyCode.F);

        input.Set(data);
    }

    // -------- CALLBACKS Empty --------
    public void OnEnable()
    {
        var r = FindObjectOfType<NetworkRunner>();
        if (r != null) r.AddCallbacks(this);
    }

    public void OnDisable()
    {
        var r = FindObjectOfType<NetworkRunner>();
        if (r != null) r.RemoveCallbacks(this);
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner r, PlayerRef p, NetworkInput i) { }
    public void OnShutdown(NetworkRunner r, ShutdownReason reason) { }
    public void OnConnectedToServer(NetworkRunner r) { }
    public void OnDisconnectedFromServer(NetworkRunner r, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner r, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner r, NetAddress ra, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner r, List<SessionInfo> sess) { }
    public void OnCustomAuthenticationResponse(NetworkRunner r, Dictionary<string, object> data) { }
    public void OnUserSimulationMessage(NetworkRunner r, SimulationMessagePtr msg) { }
    public void OnReliableDataReceived(NetworkRunner r, PlayerRef p, ArraySegment<byte> d) { }
    public void OnReliableDataReceived(NetworkRunner r, PlayerRef p, ReliableKey k, ArraySegment<byte> d) { }
    public void OnReliableDataProgress(NetworkRunner r, PlayerRef p, ReliableKey k, float prog) { }
    public void OnObjectEnterAOI(NetworkRunner r, NetworkObject o, PlayerRef p) { }
    public void OnObjectExitAOI(NetworkRunner r, NetworkObject o, PlayerRef p) { }
    public void OnHostMigration(NetworkRunner r, HostMigrationToken token) { }
}
