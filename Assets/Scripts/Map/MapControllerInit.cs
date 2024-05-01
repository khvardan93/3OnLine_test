using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Map
{
    public partial class MapController
    {
        private void ResetMap()
        {
            StopAllCoroutines();
            var data = DataUtil.Instance;

            data.AreInputsLocked = false;
            data.Score = 0;
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
                    
                    Tile tile = Tiles[x, y];
                    
                    tile.ColorIndex = colorIndex;
                    SetTileColor(Colors[colorIndex], tile.MapPosition);
                    tile.IsActive = true;
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
                    Vector3Int pos = new Vector3Int(xStartPoint + xSize, yStartPoint + ySize, 0);
                    Tilemap.SetTile(pos, TileBase);
                    Tiles[xSize, ySize] = new Tile(pos, new Vector2Int(xSize, ySize));
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
    }
}