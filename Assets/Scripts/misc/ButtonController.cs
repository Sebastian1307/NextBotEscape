using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Animación de Escala")]
    public float scaleMultiplier = 1.1f;
    public float animationSpeed = 5f;

    [Header("Paneles a activar/desactivar (opcional)")]
    public GameObject[] panelsToActivate;

    [Header("¿Este botón cierra el juego?")]
    public bool isExitButton = false;

    private Vector3 originalScale;
    private bool isHovered;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Escala suave en hover
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
            // Esto solo funciona en el editor, para simular salir
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
