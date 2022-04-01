using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProvider : MonoBehaviour
{
    [SerializeField] bool isWalkable;
    [SerializeField] bool isExplored;
    [SerializeField] bool isTeleportingTile;
    Vector2Int thisVector;
    Tile thisTile;

    private void Awake()
    {
        thisVector = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //print(thisVector);
        thisTile = new Tile(thisVector, isWalkable, isTeleportingTile);
    }
    
    public Tile GetTile()
    {
        return thisTile;
    }

    public Vector2Int ThisVector()
    {
        return thisVector;
    }
}
