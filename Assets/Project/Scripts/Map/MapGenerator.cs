using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;

namespace ItsCalls.System{
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform navmeshFlor;
    [SerializeField] private float tileSize = 1f;

    [SerializeField]private Map[] maps;
    [SerializeField]private int mapIndex;

    [Header("Map Settings")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Transform obstaclePrefab;
    [Range(0, 1)]
    [SerializeField] private float obstaclePercent;
    [SerializeField]private int _seed = 10;

    [Range(0,1)]
    [SerializeField] private float outlinePercent;
    [SerializeField] private string mapHolder;

    private Transform _tileHolder;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;
    private Coord _mapCentre;
    private Map _currentMap;
    private int maxX;
    private int maxY;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        //Cria a lista de pares de coordenadas para cada tile do mapa
        _allTileCoords = new List<Coord>();
        _currentMap = maps[mapIndex];
        
        for(int x = 0; x < _currentMap.mapSize.x; x++){
            for(int y = 0; y < _currentMap.mapSize.y; y++){
                _allTileCoords.Add(new Coord(x, y));
            }
        }

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
                Vector3 tilePosition = new Vector3(
                    -_currentMap.mapSize.x /2 + 0.5f + x, 
                    0,
                    -_currentMap.mapSize.y /2 + 0.5f + y
                );

                Transform newTile = Instantiate(_currentMap.tilePrefab, tilePosition, _currentMap.tilePrefab.rotation);
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.SetParent(_tileHolder);
            }
        }
         bool [,] obstacleMap = new bool[
            _currentMap.mapSize.x,
            _currentMap.mapSize.y
            ] ;


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
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

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

        navmeshFlor.localScale = new Vector3(
            _currentMap.mapSize.x / 10,
0,
            _currentMap.mapSize.y / 10
        ) * tileSize;


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
        -_currentMap.mapSize.x /2 + 0.5f + _x,
         0,
          -_currentMap.mapSize.y /2 + 0.5f + _y);
     return worldPos;
    }

    private void CreateOuterObstacle(int x, int y)
    {
        Vector3 pos = CoordToWorldPosition(x, y);
        Transform newObstacle = Instantiate(
            obstaclePrefab,
            pos + Vector3.up * .5f,
            Quaternion.identity
        );
        newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
        newObstacle.parent = transform.Find("Generated Map");
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
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
