using Fusion;
using UnityEngine;

public class Collectible : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Object.HasStateAuthority)
        {
            GameManager.Instance.CollectItem(transform.position);
            Runner.Despawn(Object);
        }
    }
}
