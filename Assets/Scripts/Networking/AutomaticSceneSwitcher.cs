using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticSceneSwitcher : MonoBehaviour
{
    [SerializeField] private string sceneName;
    void Start()
    {
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load scene '{sceneName}': {ex.Message}");
        }
    }
}
