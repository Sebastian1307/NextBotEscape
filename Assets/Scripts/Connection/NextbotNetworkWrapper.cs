using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class NextbotNetworkController : NetworkBehaviour
{
    public NavMeshAgent agent;
    public NextbotAI ai;

    public override void Spawned()
    {
        // Only host uses nav mesh AI
        if (!Object.HasStateAuthority)
        {
            if (agent != null) agent.enabled = false;
            if (ai != null) ai.enabled = false;
        }
    }
}
