using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayUI: MonoBehaviour
{
    [SerializeField] private Image fadeBackground;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Color fadeColor;
    [SerializeField] private float fadeDuration;

    [SerializeField] private Player player;

    private void Start()
    {
        player.OnDeath += OnGameOver;
    }

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, fadeColor, fadeDuration));
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Fade(Color from, Color to, float duration)
    {
        float speed = 1/ duration;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeBackground.color = Color.Lerp(from, to, percent);
            yield return null;
        }

        gameOverUI.SetActive(true);
    }
}