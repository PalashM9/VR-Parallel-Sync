using UnityEngine;
using Unity.Netcode;

/// <summary>
/// If isOrbiting is true and target is set,
/// this cone orbits around the target on the XZ plane.
/// Movement is driven on the SERVER only; NetworkTransform syncs it.
/// </summary>
public class OrbitingCone : MonoBehaviour
{
    [Header("Orbit Control")]
    public bool isOrbiting = false;
    public Transform target;

    public float radius = 5f;
    public float orbitSpeedDegrees = 45f;
    public float heightOffset = 0f;

    private float _angleRad;

    private void Start()
    {
        // Only the SERVER moves the cone. Clients just receive NetworkTransform updates.
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!isOrbiting) return;
        if (target == null) return;

        _angleRad += orbitSpeedDegrees * Mathf.Deg2Rad * Time.deltaTime;

        Vector3 offset = new Vector3(
            Mathf.Cos(_angleRad) * radius,
            heightOffset,
            Mathf.Sin(_angleRad) * radius
        );

        transform.position = target.position + offset;
    }
}
