using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalCollectibles = 10;
    private int collectedCount = 0;

    public event Action<int, int> OnCollectiblesUpdated;
    public event Action<Vector3> OnCollectiblePickedUp; // NUEVO

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

    public void CollectItem(Vector3 position)
    {
        collectedCount++;
        collectedCount = Mathf.Min(collectedCount, totalCollectibles);

        OnCollectiblesUpdated?.Invoke(collectedCount, totalCollectibles);
        OnCollectiblePickedUp?.Invoke(position); // Notificar posición
    }

    public (int, int) GetProgress()
    {
        return (collectedCount, totalCollectibles);
    }

    public int GetCollectedCount() => collectedCount;
}
