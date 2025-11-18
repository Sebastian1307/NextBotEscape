using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Networked] public float NetworkYaw { get; set; }

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraRoot;
    public Light flashlight;
    public Animator animator;

    private float originalHeight;
    public float crouchHeight = 1.0f;
    private Vector3 originalCameraPosition;
    public float crouchCameraOffset = -0.5f;
    public float crouchTransitionSpeed = 6f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool flashlightOn = true;
    private float currentSpeed;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();

        // Has inputAuthority controls input
        if (!Object.HasInputAuthority)
        {
            controller.enabled = false;
        }

        originalHeight = controller.height;
        originalCameraPosition = cameraRoot.localPosition;

        flashlight.enabled = flashlightOn;

        //Camera only for Local Player
        cameraRoot.gameObject.SetActive(Object.HasInputAuthority);
    }

    
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            controller.enabled = false;
        }


        if (GetInput(out PlayerInputData data))
        {
            HandleMovement(data);
            HandleFlashlight(data);
            UpdateAnimations();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetYaw(float newYaw)
    {
        NetworkYaw = newYaw;
    }

    void HandleMovement(PlayerInputData data)
    {
        // Cliente doesnt move, but needs rotation
        transform.rotation = Quaternion.Euler(0, NetworkYaw, 0);

        if (!Object.HasStateAuthority)
            return;    

        // grounded check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Speeds
        float targetSpeed = walkSpeed;
        if (data.run) targetSpeed = runSpeed;

        // Crouch
        float targetHeight = originalHeight;
        Vector3 targetCamPos = originalCameraPosition;
        float targetCenterY = controller.center.y;

        if (data.crouch)
        {
            isCrouching = true;
            targetSpeed = crouchSpeed;

            targetHeight = crouchHeight;
            targetCamPos = originalCameraPosition + new Vector3(0, crouchCameraOffset, 0);

            float heightDiff = originalHeight - crouchHeight;
            targetCenterY = heightDiff / 2f;
        }
        else
        {
            RaycastHit hit;
            bool canStand = !Physics.SphereCast(
                transform.position,
                controller.radius,
                Vector3.up,
                out hit,
                originalHeight - controller.height + 0.2f
            );

            if (canStand)
            {
                isCrouching = false;
                targetHeight = originalHeight;
                targetCamPos = originalCameraPosition;
                targetCenterY = 0f;
            }
        }

        // Collider Adjust on crouch
        Vector3 oldBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f);

        controller.height = Mathf.Lerp(controller.height, targetHeight, Runner.DeltaTime * crouchTransitionSpeed);
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, targetCenterY, 0), Runner.DeltaTime * crouchTransitionSpeed);

        Vector3 newBottom = transform.position + controller.center - Vector3.up * (controller.height / 2f);
        controller.Move(oldBottom - newBottom);

        // Camera on crouch
        cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetCamPos, Runner.DeltaTime * crouchTransitionSpeed);

        // Movement
        Vector3 move = transform.right * data.x + transform.forward * data.z;
        controller.Move(move * targetSpeed * Runner.DeltaTime);

        // Jump
        if (data.jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        // Gravity
        velocity.y += gravity * Runner.DeltaTime;
        controller.Move(velocity * Runner.DeltaTime);

        currentSpeed = move.magnitude * targetSpeed;
    }

    void HandleFlashlight(PlayerInputData data)
    {
        if (data.flashlight)
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
