using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public Vector2Int coordinates;
    public bool isWalkable;
    public bool isExplored;
    public bool isTeleportingTile;
    public bool isPath;
    public Tile connectedTo;

    public Tile(Vector2Int coordinates, bool isWalkable, bool isTeleportingTile)
    {
        this.coordinates = coordinates;
        this.isWalkable = isWalkable;
        this.isTeleportingTile = isTeleportingTile;
    }
}
