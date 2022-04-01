using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    [SerializeField] int pointsOnDestruction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Doesn't run anything if the gameobject isn't the player
        if (collision.GetComponent<Player>() != true)
            return;

        // Otherwise, the player is contacted to add the score to the screen
        gameObject.SetActive(false);
        collision.GetComponent<Player>().AddToScore(pointsOnDestruction);
        
    }
}
