using UnityEngine;

public class HostLobbyHelper: MonoBehaviour
{
    public void HostLobby()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.StartTransitionAndDelay(() =>
            {
                StartHost();
            });
            return;
        }
        else {
            StartHost();
        }
    }

    private void StartHost()
    {
        if (SteamLobby.Instance != null)
        {
            SteamLobby.Instance.HostLobby();
        }
        else
        {
            Debug.LogError("SteamLobby instance is not initialized. -> Maybe ure trying to start from the wrong Scene?");
        }
    }
}
