using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    public void StartHost()
    {
        Debug.Log("Host button clicked");
        bool ok = NetworkManager.Singleton.StartHost();
        Debug.Log("StartHost returned: " + ok);
    }

    public void StartClient()
    {
        Debug.Log("Client button clicked");
        bool ok = NetworkManager.Singleton.StartClient();
        Debug.Log("StartClient returned: " + ok);
    }
}
