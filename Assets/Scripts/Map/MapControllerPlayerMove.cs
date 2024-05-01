using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Map
{
    public partial class MapController
    {
        private void ReplaceTiles(Vector2Int pos1, Vector2Int pos2)
        {
            if(TryGetTile(pos1, out Tile tile1) && TryGetTile(pos2, out Tile tile2))
                StartCoroutine(ReplaceTilesCoroutine(tile1, tile2));
        }
        
        private void ReplaceTiles(Vector3Int pos1, Vector3Int pos2)
        {
            Tile tile1 = FindTileByPosition(pos1);
            Tile tile2 = FindTileByPosition(pos2);
            
            StartCoroutine(ReplaceTilesCoroutine(tile1, tile2));
        }
        
        private IEnumerator ReplaceTilesCoroutine(Tile tile1, Tile tile2)
        {
            int color1 = tile1.ColorIndex;
            int color2 = tile2.ColorIndex;

            tile1.ColorIndex = color2;
            SetTileColor(Colors[color2], tile1.MapPosition);

            tile2.ColorIndex = color1;
            SetTileColor(Colors[color1], tile2.MapPosition);

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
                SetTileColor(Colors[color1], tile1.MapPosition);

                tile2.ColorIndex = color2;
                SetTileColor(Colors[color2], tile2.MapPosition);
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
                DataUtil.Instance.Score++;
                tile.IsActive = false;
            }
        }

        private Tile FindTileByPosition(Vector3Int position)
        {
            Tile foundTile = Tiles.Cast<Tile>().FirstOrDefault(tile => tile.MapPosition == position);

            return foundTile;
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
    }
}
