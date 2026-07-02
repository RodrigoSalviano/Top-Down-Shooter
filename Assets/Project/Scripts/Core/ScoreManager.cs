using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score {get; private set;}

    private float _lastEnemyKillTime;
    private int _streakCount;

    [SerializeField] private float streakExpiryTime = 1f;
    [SerializeField] private int pointsForEachEnemyKilled = 5;
    [SerializeField] private int pointsStreakMutiplier = 2;

    public System.Action<int> OnScoreChanged;

    private void Start()
    {
        score = 0;
        _lastEnemyKillTime = Time.time;
        _streakCount = 0;
        Enemy.OnEnemyDeathStatic += OnEnemyKilled;
        FindFirstObjectByType<Player>().OnDeath += OnPlayerDeath;

    }

    private void OnDisable()
    {
        Enemy.OnEnemyDeathStatic -= OnEnemyKilled;
    }

    private void OnEnemyKilled()
    {
        if(Time.time < _lastEnemyKillTime + streakExpiryTime)
        {
            _streakCount ++;
        }
        else
        {
            _streakCount = 0;
        }

        _lastEnemyKillTime = Time.time;

        score += pointsForEachEnemyKilled + (int)Mathf.Pow(pointsStreakMutiplier, _streakCount);
        OnScoreChanged?.Invoke(score);
    }

    private void OnPlayerDeath()
    {
        Enemy.OnEnemyDeathStatic -= OnEnemyKilled;
    }
}