using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;
using System.Linq;

namespace Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private Tilemap Tilemap;

        private Vector2Int MapSize;
        private List<Color> Colors;
        private Tile[,] Tiles;
        public TileBase TileBase;

        private void Start()
        {
            ResetMap();
            EventUtil.Instance.OnResetMap += ResetMap;
            EventUtil.Instance.ReplaceTile += ReplaceTiles;
        }

        private void OnDestroy()
        {
            EventUtil.Instance.OnResetMap -= ResetMap;
            EventUtil.Instance.ReplaceTile -= ReplaceTiles;
        }

        private void ReplaceTiles(Vector3Int pos1, Vector3Int pos2)
        {
            Tile tile1 = FindTileByPosition(pos1);
            int color1 = tile1.ColorIndex;
            
            Tile tile2 = FindTileByPosition(pos2);
            int color2 = tile2.ColorIndex;

            tile1.ColorIndex = color2;
            SetTileColor(Colors[color2], pos1);

            tile2.ColorIndex = color1;
            SetTileColor(Colors[color1], pos2);
        }
        
        private Tile FindTileByPosition(Vector3Int position)
        {
            Tile foundTile = Tiles.Cast<Tile>().FirstOrDefault(tile => tile.Position == position);

            return foundTile;
        }

        private void SetTileColor(Color color, Vector3Int position)
        {
            Tilemap.SetTileFlags(position, TileFlags.None);
            Tilemap.SetColor(position, color);
        }
        
        #region Init
        private void ResetMap()
        {
            var data = DataUtil.Instance;
            
            MapSize = new Vector2Int(data.XDimension, data.YDimension);
            
            ClearMap();
            PaintTiles();
            
            Colors = GenerateDifferentColors(data.ColorCount);

            ColorMap();
        }
        
        private void ColorMap()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    int colorIndex = GetColorOnInit(x, y);
                    Tiles[x, y].ColorIndex = colorIndex;
                    SetTileColor(Colors[colorIndex], Tiles[x, y].Position);
                }
            }
        }

        private int GetColorOnInit(int xPos, int yPos)
        {
            int newColor = Random.Range(0, Colors.Count);

            if (CheckColorForPrevTiles(newColor, xPos, yPos))
                return newColor;

            return GetColorOnInit(xPos, yPos);
        }
        
        private bool CheckColorForPrevTiles(int colorIndex, int xPos, int yPos)
        {
            if (xPos <= 1 && yPos <= 1)
                return true;

            if (xPos > 1 && 
                Tiles[xPos - 1, yPos].ColorIndex == colorIndex &&
                Tiles[xPos - 2, yPos].ColorIndex == colorIndex)
                return false;
            
            if (yPos > 1 && 
                Tiles[xPos, yPos - 1].ColorIndex == colorIndex &&
                Tiles[xPos, yPos - 2].ColorIndex == colorIndex)
                return false;

            return true;
        }
        
        private void PaintTiles()
        {
            int xStartPoint = -Mathf.CeilToInt(MapSize.x * 0.5f);
            int yStartPoint = -Mathf.CeilToInt(MapSize.y * 0.5f);

            Tiles = new Tile[MapSize.x, MapSize.y];
            
            for (int ySize = 0; ySize < MapSize.y; ySize++)
            {
                for (int xSize = 0; xSize < MapSize.x; xSize++)
                {
                    Tilemap.SetTile(new Vector3Int(xStartPoint + xSize, yStartPoint + ySize, 0), TileBase);
                    Tiles[xSize, ySize] = new Tile(xStartPoint + xSize, yStartPoint + ySize);
                }
            }
        }

        private void ClearMap()
        {
            BoundsInt bounds = Tilemap.cellBounds;

            foreach (var position in bounds.allPositionsWithin)
            {
                Tilemap.SetTile(position, null);
            }
        }
        
        private List<Color> GenerateDifferentColors(int n)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < n; i++)
            {
                float hue = (float)i / n;
                colors.Add(Color.HSVToRGB(hue, 1f, 1f));
            }

            return colors;
        }
        #endregion
    }
}