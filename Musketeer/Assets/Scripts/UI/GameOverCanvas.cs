using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    [Header("UI элементы")]
    [Tooltip(" нопка рестарта уровн€")]
    public Button restartButton;

    [Tooltip(" нопка выхода в меню")]
    public Button exitButton;

    private void Awake()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToMenu);

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
