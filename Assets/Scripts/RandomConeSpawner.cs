using Unity.Netcode;
using UnityEngine;

public class RandomConeSpawner : NetworkBehaviour
{
    [Header("What to spawn")]
    public NetworkObject conePrefab;

    [Header("Area settings")]
    public Vector3 areaSize = new Vector3(40f, 0f, 10f); // width X and depth Z of spawn area
    public int coneCount = 15;
    public float yOffset = 0.0f; // adjust if cones float or sink

    public override void OnNetworkSpawn()
    {
        // Only the server/host decides the random positions
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

        for (int i = 0; i < coneCount; i++)
        {
            
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                0f,
                Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
            );

            Vector3 spawnPos = transform.position + randomOffset;
            spawnPos.y += yOffset; 

            // Spawn over the network
            NetworkObject cone = Instantiate(conePrefab, spawnPos, Quaternion.identity);
            cone.Spawn(true);
        }
    }

#if UNITY_EDITOR
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
#endif
}
