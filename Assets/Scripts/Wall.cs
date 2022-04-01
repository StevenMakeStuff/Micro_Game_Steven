using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip weakHitSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Weapon>())
        {
            audioSource.PlayOneShot(weakHitSound);
        }
    }
}
