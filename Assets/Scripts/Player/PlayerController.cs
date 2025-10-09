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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            targetSpeed = crouchSpeed;
        }
        else isCrouching = false;

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
