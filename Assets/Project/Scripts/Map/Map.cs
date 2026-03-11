using ItsCalls.System;
using UnityEngine;
using System;

namespace ItsCalls.System
{
    [Serializable]
    public class Map {
        public MapGenerator.Coord mapSize;

        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public Transform tilePrefab;
        public Transform obstaclePrefab;

        public MapGenerator.Coord MapCentre
        {
            get
            {
                return new MapGenerator.Coord(mapSize.x/2, mapSize.y/2);
            }
        }

    }
}