using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
   
    public static GameManager Instance { get; private set; }
  
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
    // action так как сингтон
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnGameWin;
    public event Action OnGameOver;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
   
    private void Awake()
    {
        // синглтончик
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
  
    public void WinGame()
    {
        SetState(GameState.GameWin);
        OnGameWin?.Invoke();
       //Показываем победный UI
    }

    public void LoseGame()
    {
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
        //Показываем проиггрышный UI
    }

    
    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

    switch(newState)
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
    
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SetState(GameState.Playing);
        
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        SetState(GameState.MainMenu);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SetState(GameState.Playing);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // закрываем игру даже в едиторе для красоты)
#endif
    }
}