using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovementState_Enum currentDirection = PlayerMovementState_Enum.None;
    public PlayerMovementState_Enum CurrentDirection { get { return currentDirection; } }
    [SerializeField] Vector2Int CurrentPosition;
    private bool needsCorrection;
    [Range(1f, 1000f)]
    public float playerSpeed = 1f;
    public Vector3 direction;
    public bool isSpawning = true;
    public GameManager gm;
    Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();


    void Update()
    {
        PlayerMovementState_Enum newDirection = GetInput();
        CurrentPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (!isSpawning)
        {       
            // checks to see if you can go the desired direction
                bool desiredSpaceMoveable = CheckNextSpace(newDirection);

            // Keeps the current direction of movement if not available
                if (!desiredSpaceMoveable)
                {
                    newDirection = currentDirection;
                    bool nextSpaceMoveable;
                    nextSpaceMoveable = CheckNextSpace(currentDirection);

                    if(!nextSpaceMoveable && CheckX() && CheckY())
                    {
                        currentDirection = PlayerMovementState_Enum.None;
                        needsCorrection = true;
                    }

                    else
                    {
                        currentDirection = newDirection;
                        needsCorrection = true;
                    }
                }

                // Changes direction otherwise
                else
                {
                    // Checks to see if the character can go in the new direction assigned with input. WIP Need to add in a check to see if the desired tile is walkable.
                    if (CheckOppositeDirection(newDirection))
                    {
                        currentDirection = newDirection;
                        needsCorrection = true;
                    }
                    else if (CheckX() && CheckY())
                    {
                        currentDirection = newDirection;
                        needsCorrection = true;
                    }
                }

                // Assigns the new vector
                GetDirection();

                // this section of the code keeps the player moving
                if (!needsCorrection)
                {
                    transform.position = transform.position + direction * playerSpeed * Time.deltaTime;

                }
                else
                {
                    if (currentDirection == PlayerMovementState_Enum.Up || currentDirection == PlayerMovementState_Enum.Down)
                    {
                        transform.position = new Vector3(MathF.Round(transform.position.x), transform.position.y, 0f) + direction * playerSpeed * Time.deltaTime;
                    }
                    else if (currentDirection == PlayerMovementState_Enum.Right || currentDirection == PlayerMovementState_Enum.Left)
                    {
                        transform.position = new Vector3(transform.position.x, MathF.Round(transform.position.y), 0f) + direction * playerSpeed * Time.deltaTime;
                    }
                    else
                    {
                        transform.position = new Vector3(MathF.Round(transform.position.x), MathF.Round(transform.position.y), 0f) + direction;
                    }
                    needsCorrection = false;
                }

        }

        // To quit anytime
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        else
        {
            currentDirection = PlayerMovementState_Enum.None;
        }
    }

    // Checks and returns the next location based off of whether or not it's walkable. Does so by referencing the Grid
    private bool CheckNextSpace(PlayerMovementState_Enum desiredDirection)
    {
        Vector2Int nextSpace;
        bool canMove;
        switch (desiredDirection)
        {
            case PlayerMovementState_Enum.Left:
                nextSpace = new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y);
                if (grid[nextSpace] == null)
                    return false;
                canMove = grid[nextSpace].isWalkable;
                return canMove;

            case PlayerMovementState_Enum.Right:
                nextSpace = new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y);
                if (grid[nextSpace] == null)
                    return false;
                canMove = grid[nextSpace].isWalkable;
                return canMove;

            case PlayerMovementState_Enum.Up:
                nextSpace = new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1);
                if (grid[nextSpace] == null)
                    return false;
                canMove = grid[nextSpace].isWalkable;
                return canMove;

            case PlayerMovementState_Enum.Down:
                nextSpace = new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1);
                if (grid[nextSpace] == null)
                    return false;
                canMove = grid[nextSpace].isWalkable;
                return canMove;

            case PlayerMovementState_Enum.None:
                return true;

            default:
                Debug.Log("No direction");
                return false;
        }
    }

    // Both are used to check if you're close enought to the center of a tile to turn
    private bool CheckY()
    {
        return transform.position.y % 1f <= 0.05f;
    }

    private bool CheckX()
    {
        return transform.position.x % 1f <= 0.05f;
    }

    // Allows for immediate direction change to the opposite direction.
    private bool CheckOppositeDirection(PlayerMovementState_Enum desiredDirection)
    {
        switch (desiredDirection)
        {
            case PlayerMovementState_Enum.Left:
                if (currentDirection == PlayerMovementState_Enum.Right)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case PlayerMovementState_Enum.Right:
                if (currentDirection == PlayerMovementState_Enum.Left)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case PlayerMovementState_Enum.Up:
                if (currentDirection == PlayerMovementState_Enum.Down)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case PlayerMovementState_Enum.Down:
                if (currentDirection == PlayerMovementState_Enum.Up)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case PlayerMovementState_Enum.None:
                return false;
            default:
                Debug.Log("No direction");
                return false;
        }
    }

    // returns the input direction
    private PlayerMovementState_Enum GetInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return PlayerMovementState_Enum.Up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return PlayerMovementState_Enum.Down;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return PlayerMovementState_Enum.Right;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return PlayerMovementState_Enum.Left;
        }
        else
        {
            return currentDirection;
        }
    }

    // Returns the vector
    private void GetDirection()
    {
        switch (currentDirection)
        {
            case PlayerMovementState_Enum.Left:
                direction = Vector2.left;
                break;
            case PlayerMovementState_Enum.Right:
                direction = Vector2.right;
                break;
            case PlayerMovementState_Enum.Up:
                direction = Vector2.up;
                break;
            case PlayerMovementState_Enum.Down:
                direction = Vector2.down;
                break;
            case PlayerMovementState_Enum.None:
                direction = new Vector2(0f, 0f);
                break;
            default:
                Debug.Log("No direction");
                break;
        }
    }

    public void AssignGrid(Dictionary<Vector2Int, Tile> newGrid)
    {
        grid = newGrid;
    }
}
