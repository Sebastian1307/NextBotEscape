using Fusion;
using UnityEngine;

//Nextbot sync was not working, i made this script to force update its sync, it uses the nav mesh AI only on host side, client side 
//is just a mirror of Albert position to save on ccu
public class NextbotNetworkSync : NetworkBehaviour
{
    [Networked] public Vector3 NetPos { get; set; }
    [Networked] public Quaternion NetRot { get; set; }

    public Transform targetVisual;
    public float lerpSpeed = 20f;

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            NetPos = targetVisual.position;
            NetRot = targetVisual.rotation;
        }
        else
        {
            targetVisual.position = Vector3.Lerp(
                targetVisual.position, NetPos, Runner.DeltaTime * lerpSpeed);

            targetVisual.rotation = Quaternion.Slerp(
                targetVisual.rotation, NetRot, Runner.DeltaTime * lerpSpeed);
        }
    }
}
