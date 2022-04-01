using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] List<TileProvider> tiles;
    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> Grid { get { return grid; } }
    List<Tile> walkableTiles = new List<Tile>();
    public List<Tile> WalkableTiles { get { return walkableTiles; } }

    // Used at the beginning and never again
    void Awake()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        foreach(TileProvider tile in tiles)
        {
            Vector2Int vector = tile.ThisVector();
            grid.Add(vector, tile.GetTile());
            if (grid[vector].isWalkable)
            {
                walkableTiles.Add(grid[vector]);
            }
        }
    }
}
