using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale animation")]
    public float scaleMultiplier = 1.1f;
    public float animationSpeed = 5f;

    [Header("Panels to toogle/untoggle (optional)")]
    public GameObject[] panelsToActivate;

    [Header("¿This button closes THE GAME?")]
    public bool isExitButton = false;

    private Vector3 originalScale;
    private bool isHovered;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Smooth scale on hover
        Vector3 targetScale = isHovered ? originalScale * scaleMultiplier : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isExitButton)
        {
            Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
            // This is for editor tests only
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            foreach (GameObject panel in panelsToActivate)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }
}
