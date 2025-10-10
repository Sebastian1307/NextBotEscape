using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject messagePrefab;  // Prefab del mensaje
    public Transform contentParent;   // Donde se instancian (el Content del Scroll View)
    public float minDelay = 0.5f;     // Tiempo mínimo entre mensajes
    public float maxDelay = 2f;       // Tiempo máximo entre mensajes

    private List<string> messages = new List<string>();
    private bool spawning = true;

    void Start()
    {
        LoadMessagesFromFile();
        StartCoroutine(SpawnMessagesRoutine());
    }

    void LoadMessagesFromFile()
    {
        // Lee el archivo de Resources
        TextAsset textFile = Resources.Load<TextAsset>("ChatStream");
        if (textFile != null)
        {
            string[] lines = textFile.text.Split('\n');
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    messages.Add(line.Trim());
            }
        }
        else
        {
            Debug.LogError("No se encontró el archivo ChatStream.txt en Resources");
        }
    }

    IEnumerator SpawnMessagesRoutine()
    {
        while (spawning && messages.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            SpawnRandomMessage();
        }
    }

    void SpawnRandomMessage()
    {
        int index = Random.Range(0, messages.Count);
        string line = messages[index];

        // Espera formato "usuario: mensaje"
        string[] parts = line.Split(':');
        if (parts.Length < 2) return;

        string username = parts[0].Trim();
        string messageText = line.Substring(line.IndexOf(':') + 1).Trim();

        GameObject newMsg = Instantiate(messagePrefab, contentParent);
        TMP_Text[] tmps = newMsg.GetComponentsInChildren<TMP_Text>();

        if (tmps.Length >= 2)
        {
            // Usuario
            tmps[0].text = username + ":";
            tmps[0].color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.9f, 1f);

            // Mensaje
            tmps[1].text = messageText;
        }

        // Auto scroll hacia abajo
        Canvas.ForceUpdateCanvases();
        var scroll = contentParent.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = 0f;
        }
    }
}
