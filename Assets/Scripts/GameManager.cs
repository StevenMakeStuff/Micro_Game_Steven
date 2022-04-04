using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Vector2Int playerPos;
    public Vector2Int PlayerPos { get { return playerPos; } }
    Vector2Int lastPlayerPos;
    public Vector2Int LastPlayerPos { get { return lastPlayerPos; } }
    PlayerMovementState_Enum playerDirection;
    public PlayerMovementState_Enum PlayerDirection { get { return playerDirection; } }

    [SerializeField] GridManager gridManager;
    [SerializeField] GameObject player;
    Dictionary<Vector2Int, Tile> grid;


    [SerializeField] TextMeshProUGUI scoreTotalText;
    int playerLives;
    [SerializeField] int startingLives;
    [SerializeField] TextMeshProUGUI playerLivesText;
    int playerLivesIncreaseScore;
    [SerializeField] GameObject[] enemies;
    public int totalNum;
    [SerializeField] GameObject[] pellets;
    [SerializeField] List<GameObject> newPellets = new List<GameObject>();

    [SerializeField] GameObject gameOverCanvas;

    [Range(0.1f, 5f)]
    [SerializeField] float matchResetWait = 0.5f;
    [SerializeField] Vector3 playerStartAndRespawn;
    [SerializeField] Vector2[] enemyStartAndRespawn;
    [SerializeField] Vector2Int middleEnemyAreaExit;
    [SerializeField] Vector2Int finalEnemyAreaExit;

    bool gameStart = true;
    bool gameOver = false;

    int playerScore = 0;
    private bool isPlayerLosingALife = false;
    int enemySpeedIteration = 0;
    [Tooltip("There should be 6 values stored here.")]
    [SerializeField] int[] speedIncreaseThresholdByScore;

    // sets everything up
    private void Start()
    {
        scoreTotalText.text = 0.ToString();
        playerLivesText.text = startingLives.ToString();
        playerLives = startingLives;
        grid = gridManager.Grid;
        ResetEmptyList();
        player.GetComponent<PlayerController>().AssignGrid(grid);
        TotalMatchReset();
        StartCoroutine(UpdatePlayerPos());
    }

    // Only here to be used when gameover is true
    private void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.Space))
        {
            gameOver = false;
            GameReset();
        }
        
        //Determine when the enemies cannot touch each other
        for (int i = 0; i < enemies.Length; i++)
        {
            //If enemy has touched final area exit, bool will be set to true
            if (enemies[i].transform.position.x == finalEnemyAreaExit.x && enemies[i].transform.position.y == finalEnemyAreaExit.y)
            {
                enemies[i].GetComponent<Enemy>().cannotTouchEnemies = true;
            }
        }

        //Check to see if all pellets have been collected before starting a new round
        if (totalNum == 136)
        {
            gameStart = true;
            newPellets.Clear();
            totalNum = 0;
            ResetEmptyList();
            TotalMatchReset();
        }
    }

    // used so that the enemies always know where the player is by an int vector
    public IEnumerator UpdatePlayerPos()
    {
        int x = Mathf.RoundToInt(player.transform.position.x);
        int y = Mathf.RoundToInt(player.transform.position.y);

        if(lastPlayerPos != null)
        {
            lastPlayerPos = playerPos;
        }

        else
        {
            lastPlayerPos = new Vector2Int(Mathf.RoundToInt(playerStartAndRespawn.x), Mathf.RoundToInt(playerStartAndRespawn.y));
        }

        playerPos = new Vector2Int(x, y);
        playerDirection = player.GetComponent<PlayerController>().CurrentDirection;

        // Loops itself
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(UpdatePlayerPos());
    }

    //Transfer last pellet collected from the original list into a new list
    public void AddToList(GameObject pellet)
    {
        for (int i = 0; i < newPellets.Count; i++)
        {
            //Remove empty element that occurs in the beginning
            if (i == 0)
            {
                newPellets.Add(pellet);

                if (newPellets[0] == null)
                {
                    newPellets.RemoveAt(0);
                }
            }

            //Adds the pellets to the new list
            else if (newPellets[i] == null)
            {
                newPellets.Add(pellet);
            }
        }

        //Adds up when pellet is added
        totalNum += 1;
    }

    public void IncreaseScore(int points)
    {
        playerScore += points;
        scoreTotalText.text = playerScore.ToString();
        CheckSpeedIncrease();
        playerLivesIncreaseScore += points;

        if(playerLivesIncreaseScore > 1000)
        {
            playerLivesIncreaseScore = playerLivesIncreaseScore % 1000;
            playerLives += 1;
            playerLivesText.text = playerLives.ToString();
        }
    }

    // This is for the enemies
    private void CheckSpeedIncrease()
    {
        if(playerScore > speedIncreaseThresholdByScore[enemySpeedIteration])
        {
            enemySpeedIteration++;
            if(enemySpeedIteration < speedIncreaseThresholdByScore.Length)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].GetComponent<EnemyMover>().IncreaseSpeed(enemySpeedIteration, speedIncreaseThresholdByScore.Length);
                }
            }
        }
    }

    // used so that the match reset knows that the player won't lose a life
    private void TotalMatchReset()
    {
        isPlayerLosingALife = true;
        StartCoroutine(MatchReset());
    }

    // Resets the match, not the game.
    public IEnumerator MatchReset()
    {
        if (!gameStart)
        {
            playerLives -= 1;
            playerLivesText.text = playerLives.ToString();

            totalNum = 0;
            newPellets.Clear();
            ResetEmptyList();
        }

        gameStart = false;

        // Used to exit out of coroutines
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMover>().isBeingRespawned = true;
        }

        // Resets the game
        if (playerLives <= 0)
        {
            gameOverCanvas.SetActive(true);
            gameOver = true;
        }

        else
        {
            player.GetComponent<Player>().Deactivate(false);

            yield return new WaitForSeconds(matchResetWait);

            player.GetComponent<Player>().Respawn(playerStartAndRespawn);

            if (isPlayerLosingALife)
            {
                isPlayerLosingALife = false;

                for (int i = 0; i < pellets.Length; i++)
                {
                    pellets[i].SetActive(true);
                }
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                //  All of these are being used to stop the enemies movement.
                enemies[i].GetComponent<EnemyMover>().ResetPathNumber();
                enemies[i].transform.position = enemyStartAndRespawn[i];
                enemies[i].GetComponent<EnemyMover>().isBeingRespawned = false;
                enemies[i].GetComponent<Enemy>().cannotTouchEnemies = false; //If enemy dies and respawns, this will be reset
                enemies[i].GetComponent<EnemyMover>().BeginPath(enemyStartAndRespawn[i], middleEnemyAreaExit, finalEnemyAreaExit);
            }

            for (int i = 0; i < pellets.Length; i++)
            {
                pellets[i].SetActive(true);
            }

            player.GetComponent<PlayerController>().isSpawning = false;
            player.GetComponent<Collider2D>().enabled = true;
            print("Match Reset");
        }
    }

    // Referenced and used by the enemies themselves
    public void EnemyRespawn(GameObject enemy)
    {
        int enemyNum = -1;
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] == enemy)
            {
                print("enemy found");
                enemyNum = i;
                break;
            }
        }
        if (enemyNum < 0)
        {
            print("Enemy not found");
            return;
        }

        enemy.GetComponent<EnemyMover>().isBeingRespawned = true;
        enemy.GetComponent<EnemyMover>().ResetPathNumber();
        enemy.transform.position = enemyStartAndRespawn[enemyNum];
        enemy.GetComponent<EnemyMover>().isBeingRespawned = false;
        enemy.GetComponent<EnemyMover>().BeginPath(enemyStartAndRespawn[enemyNum], middleEnemyAreaExit, finalEnemyAreaExit);
    }

    // Used so that the game doesn't crash when the enemy gets close to the player.
    // The Pathfinder script uses it
    public Vector2Int ReturnFurtherDistance(Vector2Int currentPos)
    {
        float distance1 = Mathf.Sqrt((Mathf.Pow((playerPos.x - currentPos.x), 2)) + (Mathf.Pow((playerPos.y - currentPos.y), 2)));
        float distance2 = Mathf.Sqrt((Mathf.Pow((playerPos.x - currentPos.x), 2)) + (Mathf.Pow((playerPos.y - currentPos.y), 2)));
        if (distance1 >= distance2)
        {
            return playerPos;
        }
        else
            return lastPlayerPos;
    }

    public void GameReset()
    {
        playerScore = 0;
        scoreTotalText.text = playerScore.ToString();
        playerLives = 3;
        playerLivesText.text = playerLives.ToString();
        playerLivesIncreaseScore = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyMover>().ResetSpeed();
        }
        TotalMatchReset();
        gameOverCanvas.SetActive(false);
    }

    //Reset the size of the duplicate empty list
    public void ResetEmptyList()
    {
        for (int i = 0; i < 1; i++)
        {
            newPellets.Add(null);
        }
    }
}
