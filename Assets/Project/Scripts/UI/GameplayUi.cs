using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameplayUI: MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private Player player;

    [Header("New Wave Banner")]
    [SerializeField] private RectTransform wavePanel;
    [SerializeField] private TMP_Text waveLabel;
    [SerializeField] private TMP_Text enemyCountLabel;

    [Header("Game over")]
    [SerializeField] private Image fadeBackground;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Color fadeColor;
    [SerializeField] private float fadeDuration;

    [Header("Score")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TMP_Text gameplayScoreText;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Health Bar")]
    [SerializeField] private RectTransform healthBarTransform;
    [SerializeField] private TMP_Text healthBarText;

    private Spawner _spawner;

    private int _currentScore;

    private void Awake()
    {   
        _spawner = FindAnyObjectByType<Spawner>();
        _spawner.OnNewWave += OnNewWave;
    }

    private void Start()
    {
        player.OnDeath += OnGameOver;
        player.OnHealthChange += UpadteHealthBar;
        scoreManager.OnScoreChanged += UpdateScoreText;
    }

    private void OnNewWave(int waveIndex)
    {
        waveLabel.text = "- WAVE " + waveIndex + " -";

        if (_spawner.IsCurrentWaveInfinity)
        {
            enemyCountLabel.text = "Enemies : Infinite";
        }
        else
        {
            enemyCountLabel.text = "Enemies: " + _spawner.EnemysCount;
        }

        StopCoroutine(AnimateWavePanel());
        StartCoroutine(AnimateWavePanel());
    }

    private void UpdateScoreText(int score)
    {
        _currentScore = score;

        gameplayScoreText.text = $"<size=70%>Score:</size> <color=#BFD760><b>{_currentScore:D6}</b></color>";
    }

    private void UpadteHealthBar(float health)
    {
        healthBarTransform.localScale = new Vector3(Mathf.Clamp(health / player.startingHealth, 0, 1), 1, 1);
        string newHealth = health < 0 ? "0" : health.ToString();
        healthBarText.text = newHealth + "/" + Mathf.CeilToInt(player.startingHealth).ToString();
    }

    IEnumerator AnimateWavePanel()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0f;
        int dir = 1;

        float endDelayTime = Time.time + 1.5f / speed + delayTime;

        while(animatePercent >= 0f)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if(animatePercent >= 1f)
            {
                animatePercent = 1f;

                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            
            wavePanel.anchoredPosition = Vector2.up * Mathf.Lerp(-220f, 0f, animatePercent);
            yield return null;
        } 
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