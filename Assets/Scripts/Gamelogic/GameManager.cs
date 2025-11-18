using Fusion;
using UnityEngine;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Networked] public int collectedCount { get; set; }
    public int totalCollectibles = 10;

    //Events for sync scores (even if doesnt work xd)
    public event Action<int, int> OnCollectiblesUpdated;
    public event Action<Vector3> OnCollectiblePickedUp;
    public event Action OnAllCollected;

    private int previousCollected = -1; 

    public Transform[] respawnPoints;

    public override void Spawned()
    {
        Instance = this;

        if (Object.HasStateAuthority)
        {
            collectedCount = 0;
        }

        previousCollected = collectedCount;
    }

    public void CollectItem(Vector3 position)
    {
        if (!Object.HasStateAuthority) return;

        collectedCount = Mathf.Min(collectedCount + 1, totalCollectibles);

        OnCollectiblePickedUp?.Invoke(position);
    }
    public int GetCollectedCount() => collectedCount;
    public override void FixedUpdateNetwork()
    {
        if (collectedCount != previousCollected)
        {
            previousCollected = collectedCount;

            // Theoretically, this should call itself on both host and client, but is only working on host, sorry memin xd :v
            OnCollectiblesUpdated?.Invoke(collectedCount, totalCollectibles);

            if (collectedCount >= totalCollectibles)
                OnAllCollected?.Invoke();
        }
    }
    public Vector3 GetRandomSpawnPoint() { if (respawnPoints == null || respawnPoints.Length == 0) return Vector3.zero; return respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)].position; }
}
