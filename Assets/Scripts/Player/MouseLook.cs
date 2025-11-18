using Fusion;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    public Transform cameraRoot;
    public float sensitivity = 130f;

    private float pitch;
    private float yaw;

    private PlayerController controller;

    public override void Spawned()
    {
        // Only local client handles camera
        if (!Object.HasInputAuthority)
        {
            enabled = false;
            return;
        }

        controller = GetComponent<PlayerController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (!GetInput(out PlayerInputData data)) return;

        // Calculate yaw/pitch locally
        yaw += data.mouseX * sensitivity * Runner.DeltaTime;
        pitch -= data.mouseY * sensitivity * Runner.DeltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Send YAW to PlayerController (HOST)
        controller.RPC_SetYaw(yaw);

        //Camera rotates locally
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}
