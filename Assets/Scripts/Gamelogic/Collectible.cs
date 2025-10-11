using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName = "Reliquia";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectItem(transform.position);
            Destroy(gameObject);
        }
    }
}
