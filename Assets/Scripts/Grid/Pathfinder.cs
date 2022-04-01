using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] GameManager gm;

    [SerializeField] Vector2Int startCoordinates;
    public Vector2Int StartCoordinates { get { return startCoordinates; } }
    [SerializeField] Vector2Int destinationCoordinates;
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }


    bool isAttacking = false;


    private Tile startTile;
    private Tile destinationTile;
    Tile currentSearchTile;
    List<Tile> walkableTiles = new List<Tile>();


    Queue<Tile> frontier = new Queue<Tile>();
    Dictionary<Vector2Int, Tile> reached = new Dictionary<Vector2Int, Tile>();


    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    [SerializeField] GridManager gridManager;
    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> Grid { get { return grid; } }

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            grid = gridManager.Grid;
            walkableTiles = gridManager.WalkableTiles;
        }
    }

    public List<Tile> GetNewPath(Vector2Int currentPos, EnemyBehaviorState_Enum enemyState)
    {
        ResetNodes();
        startCoordinates = currentPos;
        startTile = grid[startCoordinates];
        destinationCoordinates = GetNewDestination(enemyState);
        destinationTile = grid[destinationCoordinates];
        float distance = Mathf.Sqrt((currentPos.x - destinationTile.coordinates.x) + (currentPos.y - destinationTile.coordinates.y));

        // Was being used due to an issue with the enemy crashing when they got to close, though i came up with another solution. Keeping it here just in case.

       /* if(Mathf.Sqrt((Mathf.Pow((destinationTile.coordinates.x - currentPos.x), 2)) + (Mathf.Pow((destinationTile.coordinates.y - currentPos.y), 2))) <= 1)
        {
            frontier.Clear();
            reached.Clear();

            startTile.isPath = true;
            destinationTile.isPath = true;
            reached.Add(startTile.coordinates, startTile);
            reached.Add(destinationTile.coordinates, destinationTile);
            
        }
        else*/
        
        
        BreadthFirstSearch();
        return BuildPath();
    }

    private Vector2Int GetNewDestination(EnemyBehaviorState_Enum enemyState)
    {
        switch (enemyState)
        {
            case EnemyBehaviorState_Enum.Wandering:
                return PickFromList();

            // Never added teleportation.
            case EnemyBehaviorState_Enum.Teleporting:
            case EnemyBehaviorState_Enum.Attacking:
                isAttacking = true;
                Vector2Int newPlayerPos = gm.ReturnFurtherDistance(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
                return newPlayerPos;

            default:
                return PickFromList();
        }
    }

    // returns a random point that iswalkable
    private Vector2Int PickFromList()
    {
        int rand = UnityEngine.Random.Range(0, walkableTiles.Count);
        return walkableTiles[rand].coordinates;
    }

    // begins the search for the player or random location
    void BreadthFirstSearch()
    {
        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(startTile);
        reached.Add(startCoordinates, startTile);

        // The regular search for the tiles
        while (frontier.Count > 0 && isRunning)
        {
            currentSearchTile = frontier.Dequeue();
            currentSearchTile.isExplored = true;
            ExploreNeighbors();
            if (currentSearchTile.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }

        // This stopped the enemy from crashing the game, DO NOT REMOVE
        if (isAttacking && reached.Count < 4)
        {
            Vector2Int direction = GetDirection(gm.PlayerDirection);
            Tile neighbor = Grid[gm.PlayerPos + direction];
            if (!reached.ContainsKey(direction) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchTile;
                reached.Add(direction, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    // Used for figuring out the next location of the enemy.
    private Vector2Int GetDirection(PlayerMovementState_Enum direction)
    {
        switch (direction)
        {
            case PlayerMovementState_Enum.Left:
                return new Vector2Int(-1, 0);
            case PlayerMovementState_Enum.Right:
                return new Vector2Int(1, 0);
            case PlayerMovementState_Enum.Up:
                return new Vector2Int(0, 1);
            case PlayerMovementState_Enum.Down:
                return new Vector2Int(0, -1);
        }
        return new Vector2Int(0, 0);
    }

    // Called by the breadthfirstsearch to search all the tiles
    private void ExploreNeighbors()
    {
        List<Tile> neighbors = new List<Tile>();

        foreach(Vector2Int direction in directions)
        {
            Vector2Int searchLocation = currentSearchTile.coordinates + direction;

            if (grid.ContainsKey(searchLocation))
            {
                neighbors.Add(grid[searchLocation]);
            }
        }
        foreach(Tile neighbor in neighbors)
        {
            if(!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchTile;
                reached.Add(neighbor.coordinates, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    // builds the path to the destination and returns it.
    List<Tile> BuildPath()
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = destinationTile;

        path.Add(currentTile);
        currentTile.isPath = true;

        while (currentTile.connectedTo != null)
        {
            currentTile = currentTile.connectedTo;
            path.Add(currentTile);
            currentTile.isPath = true;
        }

        path.Reverse();

        if(isAttacking && path.Count > 5)
        {
            int numToRemove = path.Count - 5;
            path.RemoveRange(5, numToRemove);

            destinationCoordinates = path[4].coordinates;
            destinationTile = grid[destinationCoordinates];
        }

        return path;
    }


    //  Resets the grid
    private void ResetNodes()
    {
        foreach(KeyValuePair<Vector2Int, Tile> entry in grid)
        {
            entry.Value.connectedTo = null;
            entry.Value.isExplored = false;
            entry.Value.isPath = false;
        }
    }
}