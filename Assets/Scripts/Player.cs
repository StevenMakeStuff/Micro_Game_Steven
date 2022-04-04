using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gm;
    SpriteRenderer playerSprite;
    PlayerController pc;

    AudioSource audioSource;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip pelletSound;

    public bool canBeKilled = true;


    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Respawn(Vector3 respawnPos)
    {
        pc.currentDirection = PlayerMovementState_Enum.None;
        GetComponent<Collider2D>().enabled = false;
        transform.position = respawnPos;
        playerSprite.enabled = true;
        GetComponent<Collider2D>().enabled = true;
        canBeKilled = true;
    }

    public void AddToScore(int points)
    {
        gm.IncreaseScore(points);
    }

    public void Deactivate(bool isDead)
    {
        GetComponent<Collider2D>().enabled = false;
        pc.isSpawning = true;
        pc.currentDirection = PlayerMovementState_Enum.None;

        if (isDead)
        {
            print("I'm dead");
            playerSprite.enabled = false;
            StartCoroutine(gm.MatchReset());
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() && canBeKilled)
        {
            canBeKilled = false;
            audioSource.PlayOneShot(deathSound, 1f);
            //respawnBegan = true;
            collision.gameObject.GetComponent<EnemyMover>().isBeingRespawned = true;
            collision.gameObject.GetComponent<EnemyMover>().pathNumber = 0;
            Deactivate(true);
        }

        if (collision.GetComponent<Pellet>())
        {
            audioSource.PlayOneShot(pelletSound, 0.2f);
        }
    }
}
