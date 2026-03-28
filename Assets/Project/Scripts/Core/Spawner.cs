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
    private LivingEntity _playerEntity;
    private Transform _player_T;
    #endregion

    private bool _isDisable;

    void Start()
    {   
        _isDisable = false;
        _playerEntity = FindFirstObjectByType<Player>();
        _playerEntity.OnDeath += OnPlayerDeath;
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

        if(_enemysRemaining > 0 && Time.timeSinceLevelLoad > _nextSpawn)
        {
            _enemysRemaining--;
            _nextSpawn = Time.timeSinceLevelLoad + _currentWave.timeBetweenSpawn;

            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {   
        Transform tilePosition = _mapGen.GetRandomOpenTile();

        if (_isCamping)
        {
            tilePosition = _mapGen.GetTileFromPosition(_player_T.position);
        }
        
        Material tileMat = tilePosition.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;

        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;

        }

        Enemy newEnemy = Instantiate(enemyPrefab, tilePosition.position + Vector3.up, Quaternion.identity);
        newEnemy.OnDeath += OnEnemyDeath;
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