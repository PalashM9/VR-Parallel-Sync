using Unity.Netcode;
using UnityEngine;

public class PlayerTypeManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject vrPlayerPrefab;

    public static bool WantsVR = false; // set this before StartClient()

    public override void OnNetworkSpawn()
    {
        if (IsClient && IsOwner && WantsVR)
        {
            // Tell the server we want a VR player instead
            RequestSwapToVRServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSwapToVRServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        // Find that client's current player object (desktop)
        if (NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
        {
            var oldPlayer = client.PlayerObject;

            // Spawn VR player
            var vrPlayer = Instantiate(vrPlayerPrefab, oldPlayer.transform.position, oldPlayer.transform.rotation);
            vrPlayer.SpawnAsPlayerObject(clientId);

            // Despawn old desktop player
            oldPlayer.Despawn(true);
        }
    }
}
