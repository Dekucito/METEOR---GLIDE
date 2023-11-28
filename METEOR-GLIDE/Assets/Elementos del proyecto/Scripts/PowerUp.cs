using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    internal bool activeEffect;
    void OnTriggerEnter2D(Collider2D other)// Se ejecuta cuando un objeto ingresa al área de colisión
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(EffectPowerUp());
        }
    }
    private IEnumerator EffectPowerUp()// Rutina que maneja el efecto del power-up, que desactiva y activa el collider del player para hacerlo inmune
    {
        if (!activeEffect)
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();

            gameManager.textActive.SetActive(true);
            activeEffect = true;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<BoxCollider2D>().enabled = false;

            yield return new WaitForSeconds(5f);

            player.GetComponent<BoxCollider2D>().enabled = true;

            gameManager.textActive.SetActive(false);
            activeEffect = false;
        }
    }
}
