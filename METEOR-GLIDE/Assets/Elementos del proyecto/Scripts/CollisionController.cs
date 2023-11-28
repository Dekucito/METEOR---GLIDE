using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    GameObject player;
    GameManager gameManager;


    void Start()
{
    gameManager = FindAnyObjectByType<GameManager>();
    player = GameObject.FindGameObjectWithTag("Explosion");
}

void Update()
{
    // Verifica si la posicion del jugador esta fuera de los limites establecidos por el GameManager
    if (player.transform.position.x < gameManager.minX ||
        player.transform.position.x > gameManager.maxX ||
        player.transform.position.y < gameManager.minY ||
        player.transform.position.y > gameManager.maxY)
    {
        // Verifica si el jugador no esta marcado como muerto y esta vivo
        if (gameManager.playerDeadBool != true && gameManager.playerLive)
        {
            // Marca al jugador como muerto, reproduce un sonido y activa una animacion
            gameManager.playerDeadBool = true;
            audioSource.PlayOneShot(audioClip);
            player.GetComponent<Animator>().SetBool("playerDead", true);
        }
    }
}

void OnTriggerEnter2D(Collider2D other)
{
    // Verifica si el objeto que colisiona tiene la etiqueta "Player"
    if (other.CompareTag("Player"))
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.playerDeadBool = true;

        // Realiza acciones específicas segun la etiqueta del objeto que colisiona
        if (this.CompareTag("Fireball"))
        {
            StartCoroutine(FireBallCollision());
        }
        else if (this.CompareTag("FireRay"))
        {
            StartCoroutine(FireRayCollision());
        }
    }
}
private IEnumerator FireRayCollision()
{
    // Reproduce un sonido y activa una animacion en el jugador
    audioSource.PlayOneShot(audioClip);
    player.GetComponent<Animator>().SetBool("playerDead", true);
    
    yield return new WaitForSeconds(0.5f);
}

private IEnumerator FireBallCollision()
{
    // Reproduce un sonido y activa una animacion en el jugador
    audioSource.PlayOneShot(audioClip);

    // Busca el objeto del jugador por la etiqueta "Explosion" y activa una animacion
    GameObject player = GameObject.FindGameObjectWithTag("Explosion");
    player.GetComponent<Animator>().SetBool("playerDead", true);

    yield return new WaitForSeconds(0.5f);
    Destroy(this);
}
public IEnumerator LimitCorrutine()
{
    // Reproduce un sonido y activa una animación en el jugador
    audioSource.PlayOneShot(audioClip);

    // Busca el objeto del jugador por la etiqueta "Explosion" y activa una animacion
    GameObject player = GameObject.FindGameObjectWithTag("Explosion");
    player.GetComponent<Animator>().SetBool("playerDead", true);

    yield return new WaitForSeconds(0.5f);
}
}
