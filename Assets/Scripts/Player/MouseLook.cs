using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidad del mouse")]
    public float mouseSensitivity = 150f;

    [Header("Referencias")]
    public Transform cameraRoot;

    private float xRotation = 0f;

    void Start()
    {
        // Bloqueamos el cursor al centro
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical (mirar arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f); // límite para no girar 360°

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (girar cuerpo)
        transform.Rotate(Vector3.up * mouseX);
    }
}
