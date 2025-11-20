using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private GameObject uiRoot; // assign the Canvas here

    public void StartHost()
    {
        Debug.Log("Host button clicked");
        bool ok = NetworkManager.Singleton.StartHost();
        Debug.Log("StartHost returned: " + ok);

        if (ok && uiRoot != null)
            uiRoot.SetActive(false);
    }

    public void StartClient()
    {
        Debug.Log("Client button clicked");
        bool ok = NetworkManager.Singleton.StartClient();
        Debug.Log("StartClient returned: " + ok);

        if (ok && uiRoot != null)
            uiRoot.SetActive(false);
    }
}
