using UnityEngine;

public class OptionsMenuHelper : MonoBehaviour
{
    public void OpenOptionsMenu()
    {
        if (PauseMenu.Instance != null)
        {
            PauseMenu.Instance.OnOptions();
        }
        else
        {
            Debug.LogError("PauseMenu instance is not initialized. Cannot open options menu.");
        }
    }
}
