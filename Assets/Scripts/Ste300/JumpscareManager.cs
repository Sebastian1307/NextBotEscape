using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class JumpscareManager : MonoBehaviour
{
    public static JumpscareManager Instance;

    [Header("Referencias UI")]
    public Image jumpscarePanel;
    public GameObject respawnPanel;
    public TextMeshProUGUI respawntext;

    [Header("Parámetros")]
    public float flashDuration = 1f; //Flash time duration
    public float respawnDelay = 4f; // Respawn time
    public Color color1 = Color.red;
    public Color color2 = Color.white;

    private bool isActive = false;

    void Awake()
    {
        Instance = this;
        if (jumpscarePanel) jumpscarePanel.gameObject.SetActive(false);
        if (respawnPanel) respawnPanel.gameObject.SetActive(false);
    }

    public void TriggerJumpscare(PlayerController player)
    {
        if (isActive) return;
        StartCoroutine(JumpscareSequence(player));
    }

    private IEnumerator JumpscareSequence(PlayerController player)
    {
        isActive = true;

        // Deactivate camera and controls
        player.enabled = false;
        if (player.flashlight) player.flashlight.enabled = false;
        if (player.cameraRoot) player.cameraRoot.gameObject.SetActive(false);

        jumpscarePanel.gameObject.SetActive(true);
        Color startColor = color1;
        jumpscarePanel.color = startColor;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime * 10f; // Flash on cam
            jumpscarePanel.color = Color.Lerp(color1, color2, Mathf.PingPong(timer * 4f, 1f));
            yield return null;
        }

        //Black screen
        jumpscarePanel.color = Color.black;

        // Countdown visual
        if (respawnPanel)
        {
            respawnPanel.gameObject.SetActive(true);
            float t = respawnDelay;
            while (t > 0)
            {
                respawntext.text = $"Respawning in {t:0.0}s...";
                t -= Time.deltaTime;
                yield return null;
            }
            respawnPanel.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        // Player respawn
        Vector3 respawnPos = GameManager.Instance.GetRandomSpawnPoint();
        player.transform.position = respawnPos;

        // Activate cam and control again after respawn
        if (player.cameraRoot) player.cameraRoot.gameObject.SetActive(true);
        player.enabled = true;

        // Turn of Jumpscare UI panel
        jumpscarePanel.gameObject.SetActive(false);
        isActive = false;
    }
}
