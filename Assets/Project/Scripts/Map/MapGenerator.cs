using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;
using Unity.AI.Navigation;

namespace ItsCalls.System{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance;

        [SerializeField]private Map[] maps;
        [SerializeField]private int mapIndex;

        [Header("Map Settings")]
        [SerializeField] private Transform tilePrefab;
        [SerializeField] private Vector2 mapSize;
        [SerializeField] private Transform obstaclePrefab;
        [Range(0, 1)]
        [SerializeField] private float obstaclePercent;
        [SerializeField]private int _seed = 10;
        [SerializeField] private Transform outerObstaclePrefab;
        [SerializeField] private Transform groundBacking;

        [Header("Tile Settings")]
        [Range(1,10)]
        [SerializeField] private float tileSize = 1f;

        [Range(0,1)]
        [SerializeField] private float outlinePercent;
        [SerializeField] private string mapHolder;

        [Header("Nav Settings")]
        [SerializeField] private Transform navmeshFlor;
        [SerializeField] private NavMeshSurface navMeshSurface;

        [Header("Spwaner")]
        [SerializeField] private Spawner spawner;

        private Transform _tileHolder;

        private List<Coord> _allTileCoords;
        private List<Coord> _allOpenTiles;
        private Queue<Coord> _shuffledTileCoords;
        private Queue<Coord> _shuffleOpenTiles;
        private Coord _mapCentre;
        private Map _currentMap;
        private Transform[ , ] _tileMap;
        private int maxX;
        private int maxY;
        private Material groundbkMaterial;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(gameObject);
        }

        void OnEnable()
        {
            spawner.OnNewWave += OnNewWave;
        }

        void OnNewWave(int waveNumber)
        {
            mapIndex = waveNumber - 1;
            GenerateMap();
        }

        public void GenerateMap()
        {
            if(mapIndex >= maps.Length)
            {
                Debug.LogWarning("Sem mais mapas");
                return;
            }

            if(groundbkMaterial == null)
            {
             groundbkMaterial = groundBacking.GetComponent<Renderer>().sharedMaterial;   
            }
            
            _currentMap = maps[mapIndex];
            _tileMap = new Transform[_currentMap.mapSize.x, _currentMap.mapSize.y];

            //Cria a lista de pares de coordenadas para cada tile do mapa


            _allTileCoords = new List<Coord>();
            for(int x = 0; x < _currentMap.mapSize.x; x++){
                for(int y = 0; y < _currentMap.mapSize.y; y++){
                    _allTileCoords.Add(new Coord(x, y));
                }
            }

             _allOpenTiles = new List<Coord>(_allTileCoords);

            //Cria uma fila de coordenadas embaralhadas para cada tile do mapa
            _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _currentMap.seed));

            _mapCentre = _currentMap.MapCentre;

            int obstacleCount = (int)(_currentMap.mapSize.x * _currentMap.mapSize.y * _currentMap.obstaclePercent);
            int currentObstacleCount = 0;

             Transform oldMap = transform.Find(mapHolder);
            if(oldMap != null)
            {
                DestroyImmediate(oldMap.gameObject);
            }

            _tileHolder = new GameObject(mapHolder).transform;
            _tileHolder.SetParent(transform);

            for(int x = 0; x < _currentMap.mapSize.x; x++)
            {
                for(int y =0; y < _currentMap.mapSize.y; y++){
                    Vector3 tilePosition = CoordToWorldPosition(x, y);

                    Transform newTile = Instantiate(_currentMap.tilePrefab, tilePosition, _currentMap.tilePrefab.rotation);
                    newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                    newTile.SetParent(_tileHolder);

                    _tileMap[x, y] = newTile;
                }
            }
             bool [,] obstacleMap = new bool[
                _currentMap.mapSize.x,
                _currentMap.mapSize.y
                ] ;



            maxX = _currentMap.mapSize.x;
            maxY = _currentMap.mapSize.y;

            for (int x = -1; x <= maxX; x++)
            {
                CreateOuterObstacle(x, -1); 
                CreateOuterObstacle(x, maxY);
            }

            for (int y = 0; y < maxY; y++)
            {
                CreateOuterObstacle(-1, y); 
                CreateOuterObstacle(maxX, y);
            }

            
            
            for(int i = 0; i < obstacleCount; i++)
            {
                Coord randomCoord = GetRandomCoord();
                obstacleMap[randomCoord.x, randomCoord.y] = true;
                currentObstacleCount ++;

                //Validation
                if(randomCoord != _currentMap.MapCentre && MapIsFullAccessible(obstacleMap, currentObstacleCount))
                {
                    Vector3 obstaclePosition = CoordToWorldPosition(randomCoord.x, randomCoord.y);
                    Transform newObstacle = Instantiate(_currentMap.obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
                    newObstacle.SetParent(_tileHolder);
                    newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;

                    _allOpenTiles.Remove(randomCoord);
                }
                else
                {
                    obstacleMap[randomCoord.x, randomCoord.y] = false;
                    currentObstacleCount--;
                }
            }

            _shuffleOpenTiles = new Queue<Coord>(Utility.ShuffleArray(_allOpenTiles.ToArray(), _currentMap.seed));

            navmeshFlor.localScale = new Vector3(
                _currentMap.mapSize.x / 10f,
             0f,
                _currentMap.mapSize.y / 10f
            ) * tileSize;

            groundBacking.localScale = navmeshFlor.localScale + Vector3.up * 0.1f;
            groundbkMaterial.color = _currentMap.BackGroundColor;

            navMeshSurface.BuildNavMesh();
        }
        

        private bool MapIsFullAccessible(bool[,] obstacleMap, int currentObstacleCount)
        {
            bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];

            Queue<Coord> queue = new Queue<Coord>();
            queue.Enqueue(_currentMap.MapCentre);
            mapFlags[_currentMap.MapCentre.x, _currentMap.MapCentre.y] = true;
            int accessbibleTileCount = 1;

            while (queue.Count > 0)
            {
                Coord tile = queue.Dequeue();

                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                    {
                        if( x == 0 || y == 0)
                        {
                            int neighbourX = tile.x + x;
                            int neighbourY = tile.y + y;

                            if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && 
                            neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                            {
                                if(!mapFlags[neighbourX , neighbourY] && !obstacleMap[neighbourX, neighbourY])
                                {
                                    mapFlags[neighbourX, neighbourY] = true;
                                    queue.Enqueue(new Coord(neighbourX, neighbourY));
                                    accessbibleTileCount++;
                                }
                            }

                        }
                    }
                }
            }

            int targetTileCount = _currentMap.mapSize.x * _currentMap.mapSize.y - currentObstacleCount;
            return targetTileCount == accessbibleTileCount;
        }

        private Vector3 CoordToWorldPosition( int _x, int _y)
        {
         Vector3 worldPos = new Vector3(
            -_currentMap.mapSize.x /2f + 0.5f + _x,
             0,
              -_currentMap.mapSize.y /2f + 0.5f + _y)* tileSize;
         return worldPos;
        }

        public Transform GetTileFromPosition(Vector3 position)
            {
                int x = Mathf.RoundToInt(position.x / tileSize + (_currentMap.mapSize.x - 1) / 2f);
                int y = Mathf.RoundToInt(position.z / tileSize + (_currentMap.mapSize.x - 1) / 2f);

                x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
                y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);


                return _tileMap[x, y];
            }

        private void CreateOuterObstacle(int x, int y)
        {
            Vector3 pos = CoordToWorldPosition(x, y);
            Transform newObstacle = Instantiate(
                outerObstaclePrefab,
                pos + Vector3.up * .5f,
                Quaternion.identity
            );
            newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            newObstacle.parent = transform.Find("Generated Map");
            newObstacle.SetParent(_tileHolder);
        }

        private Coord GetRandomCoord()
        {
            Coord randomCoord = _shuffledTileCoords.Dequeue();
            _shuffledTileCoords.Enqueue(randomCoord);
            return randomCoord;
        }

        public Transform GetRandomOpenTile()
        {
            Coord randomCoord = _shuffleOpenTiles.Dequeue();
            _shuffleOpenTiles.Enqueue(randomCoord);
            return _tileMap[randomCoord.x, randomCoord.y];
        }


        [Serializable]
        public struct Coord
        {
            public int x;
            public int y;

            public Coord (int _x, int _y){
                x = _x;
                y = _y;
            }

            public static bool operator ==(Coord c1, Coord c2)
            {
                return c1.x == c2.x && c1.y == c2.y;
            }

            public static bool operator !=(Coord c1, Coord c2)
            {
                return c1.x != c2.x && c1.y != c2.y;
            }
        }
    }
    }
