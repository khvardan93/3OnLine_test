using System;
using System.Collections.Generic;
using System.Collections;
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
            EventUtil.Instance.ReplaceTile += ReplaceTiles;
        }

        private void OnDestroy()
        {
            EventUtil.Instance.OnResetMap -= ResetMap;
            EventUtil.Instance.ReplaceTile -= ReplaceTiles;
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
        }

        private int GetColorOnRefill(int xPos, int yPos)
        {
            int newColor = Random.Range(0, Colors.Count);

            if (CheckColorForRefill(newColor, xPos, yPos))
                return newColor;

            return GetColorOnRefill(xPos, yPos);
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

        #region Tile Fall
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
        #endregion
        
        #region Replace and destroy
        private void ReplaceTiles(Vector3Int pos1, Vector3Int pos2)
        {
            StartCoroutine(ReplaceTilesCoroutine(pos1, pos2));
        }
        
        private IEnumerator ReplaceTilesCoroutine(Vector3Int pos1, Vector3Int pos2)
        {
            Tile tile1 = FindTileByPosition(pos1);
            int color1 = tile1.ColorIndex;
            
            Tile tile2 = FindTileByPosition(pos2);
            int color2 = tile2.ColorIndex;

            tile1.ColorIndex = color2;
            SetTileColor(Colors[color2], pos1);

            tile2.ColorIndex = color1;
            SetTileColor(Colors[color1], pos2);

            if (CheckTile(tile1) || CheckTile(tile2))
            {
                DataUtil.Instance.AreInputsLocked = true;
                CheckCross(tile1);
                CheckCross(tile2);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                
                tile1.ColorIndex = color1;
                SetTileColor(Colors[color1], pos1);

                tile2.ColorIndex = color2;
                SetTileColor(Colors[color2], pos2);
            }
        }

        private void CheckCross(Tile tile)
        {
            void checkDirection(Vector2Int pos, Vector2Int dir, List<Tile> foundTiles)
            {
                Vector2Int newPos = pos + dir;

                if (newPos.x < 0 || newPos.x >= Tiles.GetLength(0) || newPos.y < 0 || newPos.y >= Tiles.GetLength(1))
                    return;

                if (Tiles[pos.x, pos.y].ColorIndex == Tiles[newPos.x, newPos.y].ColorIndex)
                {
                    foundTiles.Add(Tiles[newPos.x, newPos.y]);
                    checkDirection(newPos, dir, foundTiles);
                }
            }

            List<Tile> rightTiles = new List<Tile>();
            checkDirection(tile.ArrayPosition, Vector2Int.right, rightTiles);
            
            List<Tile> leftTiles = new List<Tile>();
            checkDirection(tile.ArrayPosition, Vector2Int.left, leftTiles);
            
            List<Tile> upTiles = new List<Tile>();
            checkDirection(tile.ArrayPosition, Vector2Int.up, upTiles);
            
            List<Tile> downTiles = new List<Tile>();
            checkDirection(tile.ArrayPosition, Vector2Int.down, downTiles);
            
            List<Tile> finalTiles = new List<Tile>();
            finalTiles.Add(tile);

            if (rightTiles.Count + leftTiles.Count + 1 >= 3)
            {
                finalTiles.AddRange(rightTiles);
                finalTiles.AddRange(leftTiles);
            }
            
            if (upTiles.Count + downTiles.Count + 1 >= 3)
            {
                finalTiles.AddRange(upTiles);
                finalTiles.AddRange(downTiles);
            }

            if (finalTiles.Count >= 3)
            {
                DestroyTiles(finalTiles);
                TileFalling();
            }
        }

        private void DestroyTiles(List<Tile> finalTiles)
        {
            foreach (var tile in finalTiles)
            {
                SetTileColor(DisabledColor, tile.MapPosition);
                tile.IsActive = false;
            }
        }

        private Tile FindTileByPosition(Vector3Int position)
        {
            Tile foundTile = Tiles.Cast<Tile>().FirstOrDefault(tile => tile.MapPosition == position);

            return foundTile;
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

        private bool CheckTile(Tile tile)
        {
            bool compareColor(Vector2Int step)
            {
                int x = tile.ArrayPosition.x + step.x;
                int y = tile.ArrayPosition.y + step.y;

                if (x < 0 || x >= Tiles.GetLength(0) || y < 0 || y >= Tiles.GetLength(1))
                    return false;
                
                return tile.ColorIndex == Tiles[x, y].ColorIndex;
            }
            
            if(compareColor(new Vector2Int(-1, 0)) && compareColor(new Vector2Int(1, 0)))
                return true;

            if(compareColor(new Vector2Int(-1, 0)) && compareColor(new Vector2Int(-2, 0)))
                return true;
            
            if(compareColor(new Vector2Int(1, 0)) && compareColor(new Vector2Int(2, 0)))
                return true;
            
            if(compareColor(new Vector2Int(0, -1)) && compareColor(new Vector2Int(0, 1)))
                return true;
            
            if(compareColor(new Vector2Int(0, -1)) && compareColor(new Vector2Int(0, -2)))
                return true;
            
            if(compareColor(new Vector2Int(0, 1)) && compareColor(new Vector2Int(0, 2)))
                return true;

            return false;
        }
        #endregion
        
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
        #endregion
    }
}