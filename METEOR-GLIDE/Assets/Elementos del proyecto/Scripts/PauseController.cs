using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;
    bool isPaused = false;
    GameManager gameManager;
    void Start()
    {
        // Busca un objeto GameManager en la escena y lo almacena para su uso posterior
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void Update()
    {
        // Lógica de actualización que maneja la pausa del juego al presionar la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.playerLive)
        {
            isPaused = !isPaused;
            HandlePause();
        }
    }
    private void HandlePause()// Si el juego está en pausa, detiene el tiempo y muestra el panel de pausa
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else 
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }
    public void Restart() //Reinicia el juego restableciendo el tiempo y ocultando el panel de pausa
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        isPaused = !isPaused;
    }
    public void Exit()// Sale del juego restableciendo el tiempo, ocultando el panel de pausa y ajustando variables del GameManager
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        gameManager.moreDifficuty = false;
        gameManager.playerLive = false;
        gameManager.CantStart = false;

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        player.cantWalk = false;

        gameManager.CancelInvoke("SpawnFireball");
        gameManager.panelMainMenu.SetActive(true);
    }
}
