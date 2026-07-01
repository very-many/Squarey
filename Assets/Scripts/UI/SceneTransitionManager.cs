using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private float transitionDelay = 0.5f;
    private static readonly int EndHash = Animator.StringToHash("End");
    private static readonly int StartHash = Animator.StringToHash("Start");

    public static SceneTransitionManager Instance { get; private set; }

    public float TransitionDelay => transitionDelay;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayTransitionStart();
    }

    public void StartTransitionAndDelay(Action onTransitionFinished)
    {
        PlayTransitionEnd();

        StartCoroutine(LoadSceneAfterTransition(onTransitionFinished));
    }

    public void PlayTransitionEnd()
    {
        if (transitionAnimation != null)
        {
            transitionAnimation.ResetTrigger(StartHash);
            transitionAnimation.SetTrigger(EndHash);
        }
    }

    public void PlayTransitionStart()
    {
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger(StartHash);
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterTransition(Action onTransitionFinished)
    {
        yield return new WaitForSeconds(transitionDelay);
        onTransitionFinished?.Invoke();
    }


}
