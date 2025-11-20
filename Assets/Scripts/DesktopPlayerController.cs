using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DesktopPlayerController : NetworkBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 120f;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
{
    if (IsServer)
    {
        // Server decides spawn positions
        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            // Host player
            transform.position = new Vector3(-2f, 0f, 0f);
        }
        else
        {
            // Any client players
            transform.position = new Vector3(2f, 0f, 0f);
        }
    }

    if (!IsOwner) return;

    // Disable existing main camera
    Camera existing = Camera.main;
    if (existing != null)
        existing.gameObject.SetActive(false);

    // Create a top-down-ish camera
    GameObject camObj = new GameObject("PlayerCamera");
    Camera cam = camObj.AddComponent<Camera>();
    cam.tag = "MainCamera";

    camObj.transform.SetParent(transform);
    camObj.transform.localPosition = new Vector3(0, 5f, -5f);
    camObj.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
}



    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up, mouseX * rotateSpeed * Time.deltaTime);

        Vector3 move = transform.forward * v + transform.right * h;
        controller.SimpleMove(move * moveSpeed);
    }
}
