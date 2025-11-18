using UnityEngine;
using TMPro;

public class UICollectorDisplay : MonoBehaviour
{
    public TextMeshProUGUI collectibleText;

    void Start()
    {
        //Coroutine for waiting to GM to instanciate, causes errors if its deleted smh?
        StartCoroutine(WaitForGM());
    }

    private System.Collections.IEnumerator WaitForGM()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.OnCollectiblesUpdated += UpdateDisplay;

        // Show starting progress
        UpdateDisplay(GameManager.Instance.collectedCount, GameManager.Instance.totalCollectibles);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollectiblesUpdated -= UpdateDisplay;
    }

    void UpdateDisplay(int collected, int total)
    {
        collectibleText.text = $"{collected} / {total}";
    }
}
