using System;
using System.ComponentModel;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private GameObject pauseMenuContainer;


    private void Start()
    {
        pauseMenuContainer = transform.GetChild(0).gameObject;  
    }

    public void OnOpenPauseMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Pause menu action triggered " + pauseMenuContainer.activeSelf);
        pauseMenuContainer.SetActive(!pauseMenuContainer.activeSelf);
    }

    public void OnResumeGame()
    {
        pauseMenuContainer.SetActive(false);
    }

    public void OnMainMenu()
    {
        GameOrchestrator.Instance.LeaveGame();
    }

    public void OnOptions()
    {
        //TODO
        throw new NotImplementedException();
    }

}
