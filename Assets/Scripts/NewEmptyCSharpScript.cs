using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class VRPlayerController : NetworkBehaviour
{
   

    public override void OnNetworkSpawn()
    {
        // Only local VR user gets XR input; others just see replicated transform.
        if (!IsOwner)
        {
            // Disable XR input components if you add your own later
            return;
        }

        // At this point XR Origin is already driving the camera & controllers.
        // Nothing special needed here for now.
    }
}
