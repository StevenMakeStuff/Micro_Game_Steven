using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Range(1f, 50f)]
    [SerializeField] float enemySpeed;
    float enemyStartingSpeed;
    [SerializeField] float enemySpeedMax;
    [SerializeField] float respawnWaitTime;
    [SerializeField] float KnockBackSpeed = .7f;


    //[SerializeField] float waitTime = 3f;
    [SerializeField] public int pathNumber = 0;
    [Range(2, 5)]
    [SerializeField] int rageNumber = 3;


    List<Tile> path = new List<Tile>();
    [SerializeField] EnemyBehaviorState_Enum enemyState = EnemyBehaviorState_Enum.Wandering;


    public bool isBeingRespawned;
    public bool isBeingKnockedBack;
    int runNumber = 0;

    Pathfinder pathFinder;

    private void OnEnable()
    {
        if(pathFinder == null)
            pathFinder = GetComponent<Pathfinder>();
    }

    private void Awake()
    {
        enemyStartingSpeed = enemySpeed;
    }

    public void BeginPath(Vector2 RespawnPos, Vector2Int firstLocation, Vector2Int secondLocation)
    {
        isBeingRespawned = false;
        isBeingKnockedBack = false;
        pathNumber = 0;
        StartCoroutine(Spawn(RespawnPos, firstLocation, secondLocation));
        //StartCoroutine(FollowPath());
    }

   public IEnumerator FollowPath()
    {
        yield return new WaitForSeconds(.01f);
        if (!isBeingRespawned && !isBeingKnockedBack)
        {
            runNumber++;
            //print($"{gameObject} is running FollowPath(). {runNumber}");
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector2Int startPosition = path[i].coordinates;
                Vector2Int endPosition = path[i + 1].coordinates;
                float travelPercent = 0f;

                while (travelPercent < 1f)
                {
                    if (isBeingRespawned || isBeingKnockedBack)
                        break;

                    travelPercent += Time.deltaTime * enemySpeed;
                    transform.position = Vector2.Lerp(startPosition, endPosition, travelPercent);
                    yield return new WaitForEndOfFrame();
                }
            }
            if (!isBeingRespawned && !isBeingKnockedBack)
            {
                FindPath();
                //StartCoroutine(FollowPath());
            }
        }
    }

    private EnemyBehaviorState_Enum DetermineState()
    {
        switch (enemyState)
        {
            case EnemyBehaviorState_Enum.Wandering:
                if (pathNumber <= 1)
                {
                    return EnemyBehaviorState_Enum.Wandering;
                }
                else if (pathNumber > rageNumber - 1)
                {
                    return EnemyBehaviorState_Enum.Attacking;
                }
                else if (UnityEngine.Random.Range(-2 + pathNumber, 5) <= rageNumber)
                {
                    return EnemyBehaviorState_Enum.Wandering;
                }
                return EnemyBehaviorState_Enum.Attacking;
            case EnemyBehaviorState_Enum.Teleporting:
                return EnemyBehaviorState_Enum.Wandering;
            case EnemyBehaviorState_Enum.Attacking:
                return EnemyBehaviorState_Enum.Attacking;
            default:
                return EnemyBehaviorState_Enum.Wandering;
        }
    }

    private void FindPath()
    {
        enemyState = DetermineState();
        pathNumber += 1;
        path.Clear();
        
        path = pathFinder.GetNewPath(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)), enemyState);
        StartCoroutine(FollowPath());
    }

    public IEnumerator Spawn(Vector2 RespawnPos, Vector2Int firstLocation, Vector2Int secondLocation)
    {
        // Here so that the enemies don't immediately spawn.
        yield return new WaitForSeconds(respawnWaitTime);

        // isBeingRespawned is spammed here in case I need to exit the coroutine early.
        if (!isBeingRespawned && !isBeingKnockedBack)
        {
            Vector2 startPosition = RespawnPos;
            Vector2Int endPosition = firstLocation;
            float travelPercent = 0f;

            // used to get to first location
            while (travelPercent < 1f)
            {
                if (isBeingRespawned || isBeingKnockedBack)
                    break;

                travelPercent += Time.deltaTime * 1;
                transform.position = Vector2.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }

            // used to get to second location
            if (!isBeingRespawned && !isBeingKnockedBack)
            {
                startPosition = endPosition;
                endPosition = secondLocation;
                travelPercent = 0f;

                while (travelPercent < 1f)
                {
                    if (isBeingRespawned || isBeingKnockedBack)
                        break;

                    travelPercent += Time.deltaTime * 0.5f;
                    transform.position = Vector2.Lerp(startPosition, endPosition, travelPercent);
                    yield return new WaitForEndOfFrame();
                }
            }
        }


        if(!isBeingRespawned && !isBeingKnockedBack)
            GetComponent<EnemyMover>().FindPath();
    }

    // So that the enemy doesn't immediately go for the player again.
    public void ResetPathNumber()
    {
        pathNumber = 0;
        enemyState = EnemyBehaviorState_Enum.Wandering;
    }

    // Sets is being knocked back first, had issues putting it at the front of the coroutine.
    public void BeginKnockBack(Vector2 direction)
    {
        isBeingKnockedBack = true;

        StartCoroutine(KnockBack(direction));
    }

    // Knocks the enemy back under specific circumstances.
    private IEnumerator KnockBack(Vector2 direction)
    {
        // Just setting up values that will be needed.
        Vector2Int throwDirection;
        Vector2 currentPosition = transform.position;
        Vector2Int CurrentPositionInInts;
        Vector2Int destinationPosition;
        float travelPercent = 0f;

        throwDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));  // This is used as a direction reference.
        CurrentPositionInInts = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));     // This is used to reference in the case of a wall behind the enemy.
        if (!isBeingRespawned)
        {
            if (pathFinder.Grid.ContainsKey(CurrentPositionInInts + throwDirection)
            && pathFinder.Grid[CurrentPositionInInts + throwDirection].isWalkable
            && !isBeingRespawned)
            {
                destinationPosition = new Vector2Int((CurrentPositionInInts.x + throwDirection.x), (CurrentPositionInInts.y + throwDirection.y));
            }

            else
            {
                destinationPosition = CurrentPositionInInts;
            }

            while (travelPercent < 1f && !isBeingRespawned)
            {
                travelPercent += Time.deltaTime * KnockBackSpeed;
                transform.position = Vector2.Lerp(currentPosition, destinationPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }

            isBeingKnockedBack = false;
        }
        
        if(!isBeingRespawned)
            FindPath();
    }

    public void IncreaseSpeed(int currentStateIndex, int totalStates)
    {
        if (enemySpeed >= enemySpeedMax)
            return;

        currentStateIndex += 1;

        // Used to calculate the new speed based off of the current speed state (implied by the currentStateIndex)
        enemySpeed = ( (currentStateIndex / totalStates) * (enemySpeedMax - enemyStartingSpeed) ) + enemyStartingSpeed;
        if (enemySpeed > enemySpeedMax)
            enemySpeed = enemySpeedMax;
    }

    public void ResetSpeed()
    {
        enemySpeed = enemyStartingSpeed;
    }
}
