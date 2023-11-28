using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiButtons : MonoBehaviour
{
    public AudioSource source;
    internal bool toggleCheckMark;


    void Start()
    {
        toggleCheckMark = true;
    }
    public void Toggle()//funcion ejecutada desde un button, que activa o desactiva el componente de audio de un 
    {
        toggleCheckMark = !toggleCheckMark;

        source.enabled = toggleCheckMark;
    }
    public void ExitButton()//funcion ejecutada desde un button,que cierra la aplicacion
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        Application.Quit();
    }
    public void OkeyButton()//funcion ejecutada desde un button, que se encarga de activar un bool para que una corrutina se ejecute
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.cantContinue = true;
    }
}
