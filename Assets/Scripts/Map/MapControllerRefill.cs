using UnityEngine;
using Utils;

namespace Map
{
    /// <summary>
    /// This part contains the logic to refill the map after all matches are destroyed
    /// </summary>
    public partial class MapController
    {
        private void RefillMap()
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (TryGetTile(x, y, out Tile tile) && !tile.IsActive)
                    {
                        tile.ColorIndex = GetColorOnRefill(x, y);
                        SetTileColor(Colors[tile.ColorIndex], tile.MapPosition);
                        tile.IsActive = true;
                    }
                }
            }

            DataUtil.Instance.AreInputsLocked = false;
        }

        private int GetColorOnRefill(int xPos, int yPos, int depth = 15)
        {
            int newColor = Random.Range(0, Colors.Count);

            if (CheckColorForRefill(newColor, xPos, yPos) || depth == 0)
                return newColor;

            return GetColorOnRefill(xPos, yPos, depth - 1);
        }
        
        private bool CheckColorForRefill(int colorIndex, int xPos, int yPos)
        {
            if (xPos <= 1 && yPos <= 1)
                return true;

            bool checkColor(Tile cTile)
            {
                return cTile.IsActive && cTile.ColorIndex == colorIndex;
            }

            if (TryGetTile(xPos - 1, yPos, out Tile testTile) && checkColor(testTile) &&
                TryGetTile(xPos + 1, yPos, out testTile) && checkColor(testTile))
                return false;
            
            if (TryGetTile(xPos - 1, yPos, out testTile) && checkColor(testTile) &&
                TryGetTile(xPos - 2, yPos, out testTile) && checkColor(testTile))
                return false;
            
            if (TryGetTile(xPos + 1, yPos, out testTile) && checkColor(testTile) &&
                TryGetTile(xPos + 2, yPos, out testTile) && checkColor(testTile))
                return false;
            
            if (TryGetTile(xPos, yPos - 1, out testTile) && checkColor(testTile) &&
                TryGetTile(xPos, yPos - 2, out testTile) && checkColor(testTile))
                return false;

            return true;
        }
    }
}
