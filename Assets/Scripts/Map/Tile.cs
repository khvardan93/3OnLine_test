using UnityEngine;

namespace Map
{
    public class Tile
    {
        public Tile(Vector3Int mapPos, Vector2Int arrayPos)
        {
            MapPosition = mapPos;
            ArrayPosition = arrayPos;

            IsActive = true;
        }
        
        public Vector3Int MapPosition;
        public Vector2Int ArrayPosition;
        public int ColorIndex;

        public bool IsActive;
    }
}