using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Environment gate that opens when any player comes close,
/// and closes when players move away.
/// - Logic runs ONLY on the server (IsServer)
/// - Movement is synced to clients via NetworkTransform
/// </summary>
public class GateReactToPlayer : NetworkBehaviour
{
    [Header("Detection")]
    [Tooltip("Distance at which the gate starts opening.")]
    public float triggerDistance = 20f;

    [Header("Movement")]
    [Tooltip("How far to move the gate when opening (world units).")]
    public Vector3 openOffset = new Vector3(0f, 4f, 0f);   // raise gate 4 units up
    [Tooltip("Speed of gate movement (units per second).")]
    public float moveSpeed = 3f;

    private Vector3 _closedPos;
    private Vector3 _openPos;

    public override void OnNetworkSpawn()
    {
        // Only server drives the motion, clients just receive NetworkTransform updates
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        _closedPos = transform.position;
        _openPos   = _closedPos + openOffset;
    }

    private void Update()
    {
        // 1. Check if any player is within trigger distance
        bool shouldOpen = false;

        DesktopPlayerController[] players = FindObjectsOfType<DesktopPlayerController>();
        foreach (var p in players)
        {
            float dist = Vector3.Distance(p.transform.position, transform.position);
            if (dist <= triggerDistance)
            {
                shouldOpen = true;
                break;
            }
        }

        // 2. Choose target position (open or closed)
        Vector3 targetPos = shouldOpen ? _openPos : _closedPos;

        // 3. Smoothly move the gate toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius in Scene view
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
#endif
}
