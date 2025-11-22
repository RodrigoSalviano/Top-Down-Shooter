using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private Enemy enemyPrefab;

    private Wave _currentWave;
    private int _currentWaveNumber;

    private int _enemysRemaining;
    private int _enemysAlive;
    private float _nextSpawn;

    void Start()
    {
        NextWave();
    }

    
    void Update()
    {
        if(_enemysRemaining > 0 && Time.timeSinceLevelLoad > _nextSpawn)
        {
            _enemysRemaining--;
            _nextSpawn = Time.timeSinceLevelLoad + _currentWave.timeBetweenSpawn;

            Enemy newEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            newEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        _enemysAlive--;
        if(_enemysAlive <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        _currentWaveNumber++;

        if(_currentWaveNumber - 1 < waves.Length)
        {
            _currentWave = waves[_currentWaveNumber - 1];
             _enemysRemaining = _currentWave.enemysCount;
            _enemysAlive = _enemysRemaining;
        }
        else
        {
            _enemysRemaining = 0;
            _enemysAlive = 0;
        }

       
    }
}

[System.Serializable]
public class Wave
{
    public int enemysCount;
    public float timeBetweenSpawn;
}