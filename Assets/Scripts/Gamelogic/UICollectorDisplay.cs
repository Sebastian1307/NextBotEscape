using UnityEngine;
using TMPro;

public class UICollectorDisplay : MonoBehaviour
{
    public TextMeshProUGUI collectibleText;

    void Start()
    {
        // Subscribirse al evento global
        GameManager.Instance.OnCollectiblesUpdated += UpdateDisplay;

        // Mostrar el progreso inicial
        var (collected, total) = GameManager.Instance.GetProgress();
        UpdateDisplay(collected, total);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblesUpdated -= UpdateDisplay;
    }

    void UpdateDisplay(int collected, int total)
    {
        collectibleText.text = $"{collected} / {total} ";
    }
}
