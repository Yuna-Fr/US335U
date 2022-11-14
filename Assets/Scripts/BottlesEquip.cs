using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlesEquip : MonoBehaviour
{
    private GameObject player;
    private PlayerController pcScript;
    private AudioSource audio;
    public AudioClip clip;

    private Vector3 distanceToPlayer;

    public int pickUpRange = 2;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        pcScript = player.GetComponent<PlayerController>();
        audio = player.GetComponent<AudioSource>();
    }
    void Update()
    {
        distanceToPlayer = player.transform.position - transform.position;
        Check();
    }

    void Check()
    {
        if (distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E))
        {
            //il récupère la bouteille
            audio.PlayOneShot(clip, 1f);
            pcScript.nbBottles++;
            Destroy(gameObject);
        }
    }
}
