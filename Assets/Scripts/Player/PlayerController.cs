using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Referencias")]
    public Transform cameraRoot;
    public Light flashlight;
    public Animator animator;


    private float originalHeight;
    public float crouchHeight = 1.0f; // altura al agacharse
    private Vector3 originalCameraPosition;
    public float crouchCameraOffset = -0.5f; // cuánto baja la cámara
    public float crouchTransitionSpeed = 6f; // suavizado


    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool flashlightOn = true;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        flashlight.enabled = flashlightOn;
        originalHeight = controller.height;
        originalCameraPosition = cameraRoot.localPosition;

    }

    void Update()
    {
        HandleMovement();
        HandleFlashlight();
        UpdateAnimations();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float targetSpeed = walkSpeed;

        // Shift = correr, Control = agacharse
        if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = runSpeed;

        // --- AGACHARSE ---
        // Calcular valores meta
        float targetHeight = originalHeight;
        Vector3 targetCamPos = originalCameraPosition;
        float targetCenterY = controller.center.y;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            targetSpeed = crouchSpeed;

            targetHeight = crouchHeight;
            targetCamPos = originalCameraPosition + new Vector3(0, crouchCameraOffset, 0);

            // Mantener pies fijos: subir centro la mitad de la diferencia
            float heightDiff = originalHeight - crouchHeight;
            targetCenterY = heightDiff / 2f;
        }
        else
        {
            RaycastHit hit;
            bool canStand = !Physics.SphereCast(transform.position, controller.radius, Vector3.up, out hit, originalHeight - controller.height + 0.2f);

            if (canStand)
            {
                isCrouching = false;
                targetHeight = originalHeight;
                targetCamPos = originalCameraPosition;
                targetCenterY = 0f; // reset al original
            }
        }

        // ⚡ APLICAR CAMBIOS MANTENIENDO LOS PIES FIJOS ⚡
        Vector3 oldBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f);

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, targetCenterY, 0), Time.deltaTime * crouchTransitionSpeed);

        // Recalcular para mantener el mismo punto inferior (pies)
        Vector3 newBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f);
        Vector3 bottomOffset = oldBottom - newBottom;
        controller.Move(bottomOffset); // desplazar collider sin hundir

        // Cámara
        cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetCamPos, Time.deltaTime * crouchTransitionSpeed);




        // Movimiento básico
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * targetSpeed * Time.deltaTime);

        // Saltar
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Guardamos velocidad actual para blend
        currentSpeed = move.magnitude * targetSpeed;
    }

    void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightOn = !flashlightOn;
            flashlight.enabled = flashlightOn;
        }
    }

    void UpdateAnimations()
    {
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        bool isJumping = !isGrounded;

        animator.SetFloat("Speed", normalizedSpeed);
        animator.SetFloat("MoveBlend", normalizedSpeed);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsJumping", isJumping);
    }
}
