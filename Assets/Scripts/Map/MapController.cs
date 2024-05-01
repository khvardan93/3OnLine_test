using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Map
{
    /// <summary>
    /// The class controls the game map
    /// </summary>
    public partial class MapController : MonoBehaviour
    {
        [SerializeField] private Tilemap Tilemap;
        [SerializeField] private TileBase TileBase;
        [SerializeField] private Color DisabledColor;
        
        private Vector2Int MapSize;
        private List<Color> Colors;
        private Tile[,] Tiles;
        private int FallCounter;
        
        private void Start()
        {
            ResetMap();
            EventUtil.Instance.OnResetMap += ResetMap;
            EventUtil.Instance.OnReplaceTile += ReplaceTiles;
            EventUtil.Instance.OnReplaceTileSim += ReplaceTiles;
        }

        private void OnDestroy()
        {
            EventUtil.Instance.OnResetMap -= ResetMap;
            EventUtil.Instance.OnReplaceTile -= ReplaceTiles;
            EventUtil.Instance.OnReplaceTileSim -= ReplaceTiles;
        }

        private bool TryGetTile(Vector2Int pos, out Tile tile)
        {
            return TryGetTile(pos.x, pos.y, out tile);
        }
        
        private bool TryGetTile(int x, int y, out Tile tile)
        {
            if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1))
            {
                tile = null;
                return false;
            }

            tile = Tiles[x, y];
            return true;
        }
        
        private void SetTileColor(Color color, int x, int y)
        {
            SetTileColor(color, new Vector3Int(x, y, 0));
        }
        
        private void SetTileColor(Color color, Vector3Int position)
        {
            Tilemap.SetTileFlags(position, TileFlags.None);
            Tilemap.SetColor(position, color);
        }
    }
}