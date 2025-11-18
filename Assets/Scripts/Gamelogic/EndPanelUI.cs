using UnityEngine;

public class EndPanelUI : MonoBehaviour
{
    public GameObject endPanel;

    void Start()
    {
        StartCoroutine(WaitForGM());
    }

    private System.Collections.IEnumerator WaitForGM()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.OnAllCollected += ShowPanel;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnAllCollected -= ShowPanel;
    }

    void ShowPanel()
    {
        endPanel.SetActive(true);
    }
}
