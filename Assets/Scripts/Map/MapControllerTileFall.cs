using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public partial class MapController
    {
        private void TileFalling()
        {
            FallCounter = 0;
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                StartCoroutine(TileFallingColumn(x));
            }
        }

        private IEnumerator TileFallingColumn(int x)
        {
            int tileCount;
            var waitABit = new WaitForSeconds(0.01f);
            
            yield return new WaitForSeconds(0.2f);;
            
            do
            {
                tileCount = 0;
                for (int y = Tiles.GetLength(1) - 2; y >= 0; y--)
                {
                    if (!Tiles[x, y].IsActive && Tiles[x, y + 1].IsActive)
                    {
                        MoveDestroyedTile(Tiles[x, y]);
                        tileCount++;

                        yield return waitABit;
                    }
                }
            } while (tileCount > 0 );

            OnFallDone();
        }

        private void OnFallDone()
        {
            FallCounter++;

            if (FallCounter == Tiles.GetLength(0))
            {
                CheckTheMap();
            }
        }

        private void MoveDestroyedTile(Tile tile)
        {
            Tile upTile = Tiles[tile.ArrayPosition.x, tile.ArrayPosition.y + 1];
            int color1 = upTile.ColorIndex;

            upTile.ColorIndex = -1;
            SetTileColor(DisabledColor, upTile.MapPosition);
            upTile.IsActive = false;

            tile.ColorIndex = color1;
            SetTileColor(Colors[color1], tile.MapPosition);
            tile.IsActive = true;
        }
        
        private void CheckTheMap()
        {
            List<Tile> foundMatches = new List<Tile>();

            void checkRow(Vector2Int startTile, Vector2Int step)
            {
                if (TryGetTile(startTile, out Tile currentTile) && currentTile.IsActive)
                {
                    List<Tile> counter = new List<Tile>();
                    Vector2Int nextPos = startTile;
                    int currentColor = -1;
                    
                    do
                    {
                        if (currentColor == -1 || currentTile.ColorIndex == currentColor)
                        {
                            counter.Add(currentTile);
                        }
                        else
                        {
                            if (counter.Count >= 3)
                                foundMatches.AddRange(counter);
                            
                            counter.Clear();
                            counter.Add(currentTile);
                        }
                        currentColor = currentTile.ColorIndex;
                        nextPos += step;
                    } while (TryGetTile(nextPos, out currentTile) && currentTile.IsActive);
                    
                    if(counter.Count >= 3)
                        foundMatches.AddRange(counter);
                }
            }

            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                checkRow(new Vector2Int(x, 0), Vector2Int.up);
            }
            
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                checkRow(new Vector2Int(0, y), Vector2Int.right);
            }

            if (foundMatches.Count >= 3)
            {
                DestroyTiles(foundMatches);
                TileFalling();
            }
            else
            {
                RefillMap();
            }
        }
    }
}
