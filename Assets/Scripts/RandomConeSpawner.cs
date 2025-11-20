using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomConeSpawner : NetworkBehaviour
{
    [Header("What to spawn")]
    public NetworkObject conePrefab;

    [Header("Area settings")]
    public Vector3 areaSize = new Vector3(40f, 0f, 10f);
    public int coneCount = 15;
    public float yOffset = 0.0f;

    [Header("Orbiting cone settings")]
    public float orbitRadius = 6f;
    public float orbitSpeedDegrees = 60f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        SpawnCones();
    }

    private void SpawnCones()
    {
        if (conePrefab == null)
        {
            Debug.LogWarning("RandomConeSpawner: conePrefab is not assigned.");
            return;
        }

        List<Transform> spawned = new List<Transform>();

        for (int i = 0; i < coneCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                0f,
                Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
            );

            Vector3 spawnPos = transform.position + randomOffset;
            spawnPos.y += yOffset;

            NetworkObject coneNO = Instantiate(conePrefab, spawnPos, Quaternion.identity);
            coneNO.Spawn(true);
            spawned.Add(coneNO.transform);
        }

        // Choose ONE cone to orbit around ANOTHER
        if (spawned.Count >= 2)
        {
            Transform center = spawned[0];   // center cone
            Transform orbiter = spawned[1];  // moving cone

            OrbitingCone orbitComp = orbiter.GetComponent<OrbitingCone>();
            if (orbitComp != null)
            {
                orbitComp.isOrbiting = true;
                orbitComp.target = center;
                orbitComp.radius = orbitRadius;
                orbitComp.orbitSpeedDegrees = orbitSpeedDegrees;
                orbitComp.heightOffset = 0f;
            }
            else
            {
                Debug.LogWarning("OrbitingCone component missing on orbiter.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }

}
