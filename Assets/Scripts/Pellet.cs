using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    [SerializeField] int pointsOnDestruction;
    public bool done;
    private GameManager gm;

    private void Start()
    {
        GameObject gameM = GameObject.FindGameObjectWithTag("GM");

        if (gameM != null)
        {
            gm = gameM.GetComponent<GameManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Doesn't run anything if the gameobject isn't the player
        if (collision.GetComponent<Player>() != true)
            return;

        // Otherwise, the player is contacted to add the score to the screen
        done = true;
        collision.GetComponent<Player>().AddToScore(pointsOnDestruction);
        gm.AddToList(gameObject);
        gameObject.SetActive(false);
    }
}
