using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverScript : MonoBehaviour
{
    public static GameOverScript Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image fadeInBlackImage;
    [SerializeField] private Image fadeOutBlackImage;
    [SerializeField] private float fadeDuration = 2f;

    void Awake()
    {
        InitializeSingleton();
        DisableUIElements();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void DisableUIElements()
    {
        fadeOutBlackImage.gameObject.SetActive(false);
        fadeInBlackImage.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    void Start()
    {
        fadeInBlackImage.color = new Color(0, 0, 0, 0);
        fadeOutBlackImage.color = new Color(0, 0, 0, 1);
    }

    public void SetGameOverText(string message)
    {
        gameOverText.text = message;
    }

    public void TriggerGameOver()
    {
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        fadeInBlackImage.gameObject.SetActive(true);
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / fadeDuration;
            fadeInBlackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        fadeOutBlackImage.gameObject.SetActive(true);
        StartCoroutine(FadeoutSecondBlackPanel());
    }

    IEnumerator FadeoutSecondBlackPanel()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeDuration;
            fadeOutBlackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeOutBlackImage.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
