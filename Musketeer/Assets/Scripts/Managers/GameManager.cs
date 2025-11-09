using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Fade Settings")]
    public Image fadeImage;       
    [SerializeField] private float fadeDuration = 1f; 

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        WaveInProgress,
        WaveCompleted,
        GameWin,
        GameOver
    }
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // events
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnGameWin;
    public event Action OnGameOver;
    public event Action OnGamePaused;
    public event Action OnGameResumed;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        InitializeFade();
    }

    private void InitializeFade()
    {
        if (fadeImage == null)
        {
            
            CreateFadeImage();
        }

        
        fadeImage.color = new Color(0, 0, 0, 0);
        StartCoroutine(FadeIn());
    }

    private void CreateFadeImage()
    {
        
        GameObject canvasGO = new GameObject("FadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        
        DontDestroyOnLoad(canvasGO);
    }

    
    public void LoadLevelWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, GameState.Playing));
    }

    public void LoadMainMenuWithFade()
    {
        StartCoroutine(LoadSceneAsync("MainMenu", GameState.MainMenu));
    }

    public void RestartLevelWithFade()
    {
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().name, GameState.Playing));
    }

    private IEnumerator LoadSceneAsync(string sceneName, GameState targetState)
    {
        
        yield return StartCoroutine(FadeOut());

        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        
        StartCoroutine(FadeIn());
        SetState(targetState);
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1f);
    }

    private IEnumerator FadeIn()
    {
        
        yield return new WaitForEndOfFrame();

        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0f); 
    }

   
    public void WinGame()
    {
        SetState(GameState.GameWin);
        OnGameWin?.Invoke();
    }

    public void LoseGame()
    {
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Paused:
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
                break;

            case GameState.Playing:
            case GameState.WaveInProgress:
            case GameState.WaveCompleted:
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
                break;

            case GameState.MainMenu:
            case GameState.GameWin:
            case GameState.GameOver:
                Time.timeScale = 1f;
                break;
        }
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Paused)
            SetState(GameState.Playing);
        else if (CurrentState == GameState.Playing || CurrentState == GameState.WaveInProgress)
            SetState(GameState.Paused);
    }

    public void QuitGame()
    {
        StartCoroutine(QuitWithFade());
    }

    private IEnumerator QuitWithFade()
    {
        yield return StartCoroutine(FadeOut());
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}