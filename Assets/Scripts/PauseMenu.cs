using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private GameObject backgroundContainer;
    private List<GameObject> menuContainers = new List<GameObject>();
    private readonly Stack<GameObject> menuStack = new Stack<GameObject>();

    public static PauseMenu Instance { get; private set; }

    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        backgroundContainer = transform.GetChild(0).gameObject;
        for (int i = 0; i < backgroundContainer.transform.childCount; i++)
        {
            menuContainers.Add(backgroundContainer.transform.GetChild(i).gameObject);
        }

        InitializeMenuState();

    }

    private void InitializeMenuState()
    {
        backgroundContainer.SetActive(false);
        menuStack.Clear();

        for (int i = 0; i < menuContainers.Count; i++)
        {
            menuContainers[i].SetActive(false);
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (menuStack.Count == 0)
        {
            EnablePauseMenu();
            return;
        }

        if (menuStack.Count == 1)
        {
            DisablePauseMenu();
            return;
        }

        CloseCurrentMenu();
    }

    private void EnablePauseMenu()
    {
        backgroundContainer.SetActive(true);
        PushMenu(menuContainers[0]);
        if (GameOrchestrator.Instance != null) GameOrchestrator.Instance.DisableLocalPlayerInput();
    }

    private void DisablePauseMenu()
    {
        while (menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false);
        }

        backgroundContainer.SetActive(false);
        if (GameOrchestrator.Instance != null) GameOrchestrator.Instance.EnableLocalPlayerInput();
    }

    private void EnableSublevelMenu(int menuIndex)
    {
        if (menuIndex < 0 || menuIndex >= menuContainers.Count)
        {
            return;
        }

        if (menuStack.Count == 0)
        {
            EnablePauseMenu();
        }

        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(false);
        }

        PushMenu(menuContainers[menuIndex]);
    }

    private void DisableSublevelMenu(int menuIndex)
    {
        if (menuIndex < 0 || menuIndex >= menuContainers.Count || menuStack.Count == 0)
        {
            return;
        }

        menuContainers[menuIndex].SetActive(false);

        if (menuStack.Count > 0)
        {
            menuStack.Pop();
        }

        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(true);
        }
        else
        {
            backgroundContainer.SetActive(false);
            if (GameOrchestrator.Instance != null) GameOrchestrator.Instance.EnableLocalPlayerInput();
        }
    }

    private void PushMenu(GameObject menu)
    {
        menu.SetActive(true);
        menuStack.Push(menu);
    }

    private void CloseCurrentMenu()
    {
        if (menuStack.Count <= 1)
        {
            return;
        }

        GameObject currentMenu = menuStack.Pop();
        currentMenu.SetActive(false);
        menuStack.Peek().SetActive(true);
    }

    public void OnResumeGame()
    {
        DisablePauseMenu();
    }

    public void OnMainMenu()
    {
        if (GameOrchestrator.Instance != null) GameOrchestrator.Instance.LeaveGame();
        else
        {
            if(Application.isEditor) UnityEditor.EditorApplication.ExitPlaymode();
            else Application.Quit();
        }
    }

    public void OnOptions()
    {
        EnableSublevelMenu(1);
    }

}
