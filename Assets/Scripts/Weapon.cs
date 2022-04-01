using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] float projectileSpeed;
    Vector3 shotDirection = new Vector2(0,0);
    Player player;


    private void Update()
    {
        transform.position = transform.position + shotDirection * projectileSpeed * Time.deltaTime;
    }

    // in case it doesn't die on its own.
    private IEnumerator WaitTillDeath()
    {
        yield return new WaitForSeconds(8f);

        Destroy(gameObject);
    }

    // begins the flight
    public void Shoot(PlayerMovementState_Enum direction, Player playerRef)
    {
        StartCoroutine(WaitTillDeath());

        player = playerRef;

        //Vector2 projectileDirection = new Vector2();

        switch (direction)
        {
            case PlayerMovementState_Enum.Up:
                shotDirection = Vector2.up;
                break;
            case PlayerMovementState_Enum.Down:
                shotDirection = Vector2.down;
                break;
            case PlayerMovementState_Enum.Left:
                shotDirection = Vector2.left;
                transform.Rotate(0f, 0f, 90f);
                break;
            case PlayerMovementState_Enum.Right:
                shotDirection = Vector2.right;
                transform.Rotate(0f, 0f, 90f);
                break;
            default:
                transform.position = new Vector3(-10000f, -10000f, 0f);
                shotDirection = Vector2.down;
                break;
        }
        //rb2d.AddForce(projectileDirection);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<Enemy>() && !collision.GetComponent<EnemyMover>().isBeingRespawned)
        {
            collision.GetComponent<Enemy>().ReduceHealth(shotDirection, player);

            StopCoroutine(WaitTillDeath());

            Destroy(gameObject);
        }

        else if (collision.GetComponent<Wall>())
        {
            StopCoroutine(WaitTillDeath());

            Destroy(gameObject);
        }
    }
}
