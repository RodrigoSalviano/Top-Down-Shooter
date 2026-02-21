using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Transform obstaclePrefab;

    [Range(0,1)]
    [SerializeField] private float outlinePercent;
    [SerializeField] private string mapHolder;

    private Transform _tileHolder;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;
    private int _seed = 10;

    void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        _allTileCoords = new List<Coord>();
        for(int x = 0; x < mapSize.x; x++){
            for(int y = 0; y < mapSize.y; y++){
                _allTileCoords.Add(new Coord(x, y));
            }
        }

        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _seed));

        int obstacleCount = 10;

         Transform oldMap = transform.Find(mapHolder);
        if(oldMap != null)
        {
            DestroyImmediate(oldMap);
        }

        _tileHolder = new GameObject(mapHolder).transform;
        _tileHolder.SetParent(transform);


        for(int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToWorldPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
            newObstacle.parent = _tileHolder;
        }



        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y =0; y < mapSize.y; y++){
                Vector3 tilePosition = new Vector3(
                    -mapSize.x /2 + 0.5f + x, 
                    0,
                    -mapSize.y /2 + 0.5f + y
                );

                Transform newTile = Instantiate(tilePrefab, tilePosition, tilePrefab.rotation);
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.SetParent(_tileHolder);
            }
        }
    }

    private Vector3 CoordToWorldPosition( int _x, int _y)
    {
     Vector3 worldPos = new Vector3(
        -mapSize.x /2 + 0.5f + _x,
         0,
          -mapSize.y /2 + 0.5f + _y);
     return worldPos;
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord (int _x, int _y){
            x = _x;
            y = _y;
        }
    }
}
