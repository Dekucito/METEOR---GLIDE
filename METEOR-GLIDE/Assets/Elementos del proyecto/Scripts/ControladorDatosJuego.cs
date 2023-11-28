using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ControladorDatosJuego : MonoBehaviour
{
    public DatosJuegos datosJuego = new DatosJuegos();
    public string archivoGuardado;
    public GameManager gameManager;

    private void Awake()
    {
        // Establece la ruta del archivo de guardado en la carpeta "Assets" del proyecto
        archivoGuardado = Application.dataPath + "/datosJuego.Json";
    }
    public void CargarDatos()// Carga los datos del juego desde el archivo JSON si existe
    {
        if (File.Exists(archivoGuardado))
        {
            string contenido = File.ReadAllText(archivoGuardado);
            datosJuego = JsonUtility.FromJson<DatosJuegos>(contenido);

            gameManager.highestScore = datosJuego.hightScore;
        }
        else
        {
            Debug.Log("el archivo no existe");
        }
    }
    public void GuardadoDatos()// Guarda los datos actuales del juego en un archivo JSON
    {
        DatosJuegos nuevosDatos = new DatosJuegos()
        {
            hightScore = gameManager.highestScore,
        };

        string cadenaJSON = JsonUtility.ToJson(nuevosDatos);

        File.WriteAllText(archivoGuardado, cadenaJSON);

        Debug.Log("archivo guardado");
    }
}