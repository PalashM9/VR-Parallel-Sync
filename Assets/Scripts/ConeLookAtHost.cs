using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Rotates the cone's pointer child on Y-axis to match the host player's forward direction.
/// </summary>
public class ConeLookAtHost : NetworkBehaviour
{
    [Header("Rotation")]
    public float turnSpeedDegreesPerSecond = 150f;

    [Header("Visual")]
    // Assign the Pointer child here in the prefab
    public Transform pointer;

    private Transform hostTransform;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        // keep trying until host exists
        InvokeRepeating(nameof(TryFindHost), 0f, 1f);
    }

    private void TryFindHost()
    {
        if (hostTransform != null) return;
        if (NetworkManager == null) return;

        ulong hostId = NetworkManager.ServerClientId;

        DesktopPlayerController[] players = FindObjectsOfType<DesktopPlayerController>();
        foreach (var p in players)
        {
            if (p.OwnerClientId == hostId)
            {
                hostTransform = p.transform;
                Debug.Log("ConeLookAtHost: Found host player for cone.");
                CancelInvoke(nameof(TryFindHost));
                break;
            }
        }
    }

    private void Update()
{
    if (!IsServer) return;
    if (pointer == null) return;

    
    pointer.Rotate(0f, 120f * Time.deltaTime, 0f, Space.Self);

    
    if (hostTransform != null)
    {
        Vector3 toHost = hostTransform.position - pointer.position;
        toHost.y = 0f;

        if (toHost.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(toHost);
            pointer.rotation = Quaternion.RotateTowards(
                pointer.rotation,
                target,
                150f * Time.deltaTime
            );
        }
    }
}

}
