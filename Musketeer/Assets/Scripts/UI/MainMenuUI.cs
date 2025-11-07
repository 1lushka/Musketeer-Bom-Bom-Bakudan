using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Кнопки")]
    public Button startButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Панели")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    private void Start()
    {
        startButton?.onClick.AddListener(StartGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        exitButton?.onClick.AddListener(ExitGame);

        ShowMainMenu();
    }

    void StartGame()
    {
        LoadGameWithFade();
    }

    void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    void ExitGame()
    { 
        GameManager.Instance.QuitGame();
    }

    void LoadGameWithFade()
    {
        GameManager.Instance.LoadLevel("GameScene"); 
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}