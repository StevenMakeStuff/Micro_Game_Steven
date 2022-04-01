using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int health = 1;
    int currentGameHealth = 1;
    [SerializeField] int maxHealth = 5;
    EnemyMover enemyMover;
    AudioSource audioSource;
    [Tooltip("Set before you try a game.")]
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] int pointsOnDestruction;
    private bool isBeingHit;


    private void Awake()
    {
        enemyMover = GetComponent<EnemyMover>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<Weapon>() && !enemyMover.isBeingRespawned)
        {
            audioSource.PlayOneShot(hurtSound);
        }

            // stops enemy from moving after impact
        if (collision.GetComponent<Player>())
        {
            StopAllCoroutines();
        }
    }

    //
    public void ReduceHealth(Vector3 direction, Player player)
    {
        if (!isBeingHit)
        {
            isBeingHit = true;
            health -= 1;
            if (health > 0)
            {
                enemyMover.BeginKnockBack(direction);
            }
            else
            {
                player.AddToScore(pointsOnDestruction);
                IncreaseHealthMax();
                enemyMover.isBeingRespawned = true;
                enemyMover.StopAllCoroutines();
                gameManager.EnemyRespawn(gameObject);
            }

            isBeingHit = false;
        }
    }

    public void IncreaseHealthMax()
    {
        if (currentGameHealth < maxHealth)
        {
            currentGameHealth++;
            health = currentGameHealth;
        }
        else
            health = maxHealth;
    }

}
