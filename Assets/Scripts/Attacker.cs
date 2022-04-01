using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    bool canAttack = true;
    [SerializeField] float attackAgainWaitTime = .7f;
    [SerializeField] GameObject weapon;
    AudioSource audioSource;
    [SerializeField] AudioClip shootingSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerMovementState_Enum direction = GetComponent<PlayerController>().CurrentDirection;
            if (canAttack && direction != PlayerMovementState_Enum.None)
            {
                canAttack = false;
                StartCoroutine(WaitToFire());
                audioSource.PlayOneShot(shootingSound);
                GameObject instance = Instantiate(weapon, transform.position, Quaternion.identity);
                instance.GetComponent<Weapon>().Shoot(direction, GetComponent<Player>());
            }
        }
    }

    private IEnumerator WaitToFire()
    {
        yield return new WaitForSeconds(attackAgainWaitTime);

        canAttack = true;
    }
}