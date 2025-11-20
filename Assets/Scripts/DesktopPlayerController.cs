using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DesktopPlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Fallback Spawn (only if tags not found)")]
    public Vector3 hostSpawnPosition   = new Vector3(48.27f, 1.51f, -0.61f);
    public Vector3 clientSpawnPosition = new Vector3(55.1f,  1.51f, -4.39f);
    public float startYaw = 0f;

    [Header("Camera")]
    public Vector3 cameraOffset = new Vector3(0f, 3f, -6f);

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        ulong serverId = NetworkManager.ServerClientId;
        bool isHostPlayer = OwnerClientId == serverId;

        // ---------- SERVER: choose spawn ----------
        if (IsServer)
        {
            Transform spawnTransform = null;

            if (isHostPlayer)
            {
                GameObject hostSpawn = GameObject.FindWithTag("HostSpawn");
                if (hostSpawn != null) spawnTransform = hostSpawn.transform;
            }
            else
            {
                GameObject clientSpawn = GameObject.FindWithTag("ClientSpawn");
                if (clientSpawn != null) spawnTransform = clientSpawn.transform;
            }

            Vector3 pos;
            Quaternion rot;

            if (spawnTransform != null)
            {
                pos = spawnTransform.position;
                rot = spawnTransform.rotation; // <- uses your spawn Y rotation
            }
            else
            {
                pos = isHostPlayer ? hostSpawnPosition : clientSpawnPosition;
                rot = Quaternion.Euler(0f, startYaw, 0f);
            }

            transform.SetPositionAndRotation(pos, rot);
            Debug.Log($"Spawning {(isHostPlayer ? "HOST" : "CLIENT")} at {pos}");
        }

        // ---------- LOCAL OWNER: attach camera ----------
        if (!IsOwner) return;

        Camera existing = Camera.main;
        if (existing != null)
            existing.gameObject.SetActive(false);

        GameObject camObj = new GameObject("PlayerCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";

        camObj.transform.SetParent(transform);
        camObj.transform.localPosition = cameraOffset;

        // Look along the capsule's forward direction
        Vector3 lookTarget = transform.position + transform.forward * 10f;
        camObj.transform.LookAt(lookTarget, Vector3.up);
    }

    void Update()
{
    if (!IsOwner) return;   // only local player

    float h = 0f;
    float v = 0f;

    // Accept BOTH WASD and Arrow keys
    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  h -= 1f;
    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;
    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    v += 1f;
    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  v -= 1f;

    // DEBUG: see if this ever fires on the client
    if ((h != 0f || v != 0f) && Input.anyKey)
    {
        Debug.Log($"MOVE INPUT on {(IsServer ? "HOST" : "CLIENT")}  h={h}, v={v}");
    }

    Vector3 dir = (transform.forward * v + transform.right * h).normalized;
    controller.Move(dir * moveSpeed * Time.deltaTime);
}


}
