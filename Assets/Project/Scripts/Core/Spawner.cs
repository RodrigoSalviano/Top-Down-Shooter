using System;
using System.Collections;
using ItsCalls.System;
using UnityEngine;

public class Spawner : MonoBehaviour
{   [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float spawnDelay = 1;

    [Header("Spawn Preview Setting")]
    [SerializeField] private float tileFlashSpeed = 4;
    [SerializeField] private Color flashColor;

    #region Wave Control Properties
    private Wave _currentWave;
    private int _currentWaveNumber;
    #endregion

    #region Enemies Control Properties
    private int _enemysRemaining;
    private int _enemysAlive;
    private float _nextSpawn;
    #endregion

    #region Camping Control Properties
    private float _timeBetweenCampingCheck = 2f;
    private float _nextCampingCheckTime;
    private float _thresholdDistance = 1.5f;
    private Vector3 _campLastPosition;
    private bool _isCamping;    
    #endregion

    #region References
    private MapGenerator _mapGen;
    private Player _playerEntity;
    private Transform _player_T;
    #endregion

    public event Action<int> OnNewWave;

    private bool _isDisable;
    private Coroutine _spawnCoroutine;

    void Start()
    {   
        _isDisable = false;
        _playerEntity = FindFirstObjectByType<Player>();
        _playerEntity.OnDeath += OnPlayerDeath;
        _playerEntity.OnDebugNextWave += HandleDebugNextWave;
        _player_T = _playerEntity.transform;

        _nextCampingCheckTime = Time.time + _timeBetweenCampingCheck;
        _campLastPosition = _player_T.position;

        _mapGen = MapGenerator.Instance;
        NextWave();
    }

    void OsDisable()
    {
        _playerEntity.OnDeath -= OnPlayerDeath;
    }


    void Update()
    {
        if (_isDisable) return;

        if(Time.time > _nextCampingCheckTime)
        {
            _nextCampingCheckTime = Time.time + _timeBetweenCampingCheck;

            _isCamping = Vector3.Distance(_player_T.position, _campLastPosition) < _thresholdDistance;

            _campLastPosition = _player_T.position;
        }

        if((_enemysRemaining > 0 || _currentWave.isInfinit)&& Time.timeSinceLevelLoad > _nextSpawn)
        {
            _enemysRemaining--;
            _nextSpawn = Time.timeSinceLevelLoad + _currentWave.timeBetweenSpawn;

            _spawnCoroutine = StartCoroutine(SpawnEnemy());
        }
    }

    void ResetPlayerPosition()
    {
        _player_T.position = _mapGen.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    IEnumerator SpawnEnemy()
    {   
        Transform tilePosition = _mapGen.GetRandomOpenTile();

        if (_isCamping)
        {
            tilePosition = _mapGen.GetTileFromPosition(_player_T.position);
        }
        
        Material tileMat = tilePosition.GetComponent<Renderer>().material;
        Color initialColor = Color.white;

        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;

        }

        Enemy newEnemy = Instantiate(enemyPrefab, tilePosition.position + Vector3.up, Quaternion.identity);
        newEnemy.OnDeath += OnEnemyDeath;
        newEnemy.SetCharacterstics(_currentWave.movespeed, _currentWave.turnSpeed, _currentWave.hitsToKillPlayer, _currentWave.enemyHealth, _currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        _isDisable = true;
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

            OnNewWave?.Invoke(_currentWaveNumber);
            ResetPlayerPosition();
        }
        
    }

    private void HandleDebugNextWave()
    {
        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;

        foreach(Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }
        NextWave();
    }
}


[System.Serializable]
public class Wave
{
    public bool isInfinit;
    public int enemysCount;
    public float timeBetweenSpawn;
    public float movespeed;
    public float turnSpeed;
    public int hitsToKillPlayer;
    public float enemyHealth;
    public Color skinColor;
}