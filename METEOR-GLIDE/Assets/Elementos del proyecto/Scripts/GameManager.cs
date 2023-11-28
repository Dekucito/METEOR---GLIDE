using System.Collections;
using UnityEngine;
using TMPro;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;
using Quaternion = UnityEngine.Quaternion;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int actualScore;
    public int highestScore;
    public int scorePlus;
    public TMP_Text[] scoreText;
    public TMP_Text[] HighScoreText;

    public bool playerLive;
    [Header("Map Limit")]
    public float offset;
    public GameObject player;
    internal float minX, minY, maxX, maxY;
    Camera mainCamera;
    [Header("Fire Ball")]
    public GameObject fireballPrefab;
    public float spawnInterval;
    public float fireBallSpeed;
    [Header("difficulty curve")]
    [SerializeField]
    float pasteTime = 0;
    public float initialCountDifficultyUpgrade,intialTimeForTheNextUpgrade,initialSpawnInterval;
    [SerializeField]
    int countDifficultyUpgrade;
    [SerializeField]
    float timeForNextUpgrade;
    public bool moreDifficuty;
    [Header("Game Data")]
    public ControladorDatosJuego gameController;
    [Header("fireRay")]
    public bool CantStart;
    public GameObject[] objetos;
    public GameObject objetoActivoAnterior;
    [SerializeField]
    float fireRayTimerForNext,initialFireRayTimerForNext;
    [Header("Game Events")]
    public bool playerDeadBool;
    internal bool cantContinue;
    [Header("UI Objects")]
    public GameObject panelMainMenu;
    public GameObject dialoguePanel;
    public GameObject panelDeadMenu;
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip audioClip;
    [Header("PowerUp Effects")]
    public GameObject powerUpPrefab;
    float spawnPowerUp;
    [SerializeField]
    internal GameObject textActive;

    #region LifeCycle
    void Start()
    {
        gameController.CargarDatos();//carga datos del juego, si estos existen

        panelMainMenu.SetActive(true);

        // Configuración inicial de elementos del juego y límites del mapa
        moreDifficuty = false;
        playerLive = false;
        CantStart = false;
        cantContinue = false;

        RestartValues();// resetea los valores del juego

         // Establece el puntaje más alto en los textos correspondientes
        for (int i = 0; i < HighScoreText.Length; i++)
        {
            HighScoreText[i].text = highestScore.ToString();
        }

        mainCamera = Camera.main;
        CheckLimits();// revisa los limites del mapa
    }
    void FixedUpdate()// Actualización periódica basada en el tiempo fijo para el puntaje y visualización de límites
    {
        if (playerLive)
        {
            actualScore += scorePlus;
            for (int i = 0; i < scoreText.Length; i++)
            {
                scoreText[i].text = actualScore.ToString();
            }
        }
        else
        {
            UpdateHighScores(actualScore);
        }
    }
    void Update()// Actualización continua del juego para la curva de dificultad, manejo de muerte del jugador y generación de FireRay
    {
        DifficultyCurve();
        PlayerDead();
        PlaySpawnFireRay();  

        // Controla el tiempo de generación de PowerUps
        spawnPowerUp += Time.deltaTime;
        if (spawnPowerUp >= 20 && playerLive)
        {
            PowerUpSpawn();
            spawnPowerUp = 0;
        }
    }
#endregion
    #region LimitsMap
    private void CheckLimits()// Configura los límites del mapa en función de la vista de la cámara
    {
        minX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + offset;
        maxX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - offset;
        minY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + offset;
        maxY = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - offset;
    }
#endregion
    #region Score
    private void UpdateHighScores(int newScore)// Actualiza el puntaje más alto y lo guarda si es necesario
    {
        if (actualScore > highestScore)
        {
            highestScore = newScore;
            gameController.GuardadoDatos();
        }
        for (int i = 0; i < HighScoreText.Length; i++)
        {
            HighScoreText[i].text = highestScore.ToString();
        }
    }
#endregion
    #region SpawnAndAtacksValue
    void SpawnFireball()// Genera un Fireball en una posición aleatoria
    {
        float axis = Random.Range(0f, 1f);
        float spawnX, spawnY;

        // Lógica para determinar la posición de generación del Fireball
        if (axis < 0.5f)// generacion horizontal
        {
            spawnX = Random.Range(minX, maxX);
            spawnY = (Random.Range(0f, 1f) < 0.5f) ? minY - offset : maxY + offset;
        }
        else // genracion vertical
        {
            spawnY = Random.Range(minY, maxY);
            spawnX = (Random.Range(0f, 1f) < 0.5f) ? minX - offset : maxX + offset;
        }
             // Instancia el Fireball y le aplica una velocidad en dirección al centro
            Instantiate(fireballPrefab, new Vector2(spawnX, spawnY), Quaternion.identity)
            .GetComponent<Rigidbody2D>().velocity =
            ((Vector2)transform.position - new Vector2(spawnX, spawnY)).normalized * fireBallSpeed;
    }
    void PlaySpawnFireRay()// Controla la generación periódica de FireRay
    {
        if (CantStart)
        {
            fireRayTimerForNext -= Time.deltaTime;

            if (fireRayTimerForNext <= 0)
            {
                SpawnFireRay();

                initialFireRayTimerForNext = Random.Range(10, 20);
                fireRayTimerForNext = initialFireRayTimerForNext;

                StartCoroutine(FireRayRutine());
            }
        }
    }
    void SpawnFireRay()// Genera un FireRay activando un objeto aleatorio de la lista de objetos
    {
        if (objetoActivoAnterior != null)
        {
            objetoActivoAnterior.SetActive(false);
        }

        int indiceAleatorio = Random.Range(0, objetos.Length);

        objetoActivoAnterior = objetos[indiceAleatorio];
        objetoActivoAnterior.SetActive(true);

        StartCoroutine(FireRayRutine());
    }
    IEnumerator FireRayRutine()// Rutina para la animación y desactivación de FireRay
    {
        Animator anim = objetoActivoAnterior.GetComponent<Animator>();
        anim.SetBool("playerAnim",true);

        yield return new WaitForSeconds(4f);

        anim.SetBool("playerAnim",false);

        yield return new WaitForSeconds(5f);

        objetoActivoAnterior.SetActive(false);
    }
    void DifficultyCurve()// Ajusta la curva de dificultad del juego
    {
        if (moreDifficuty)
        {
            pasteTime += Time.deltaTime;

            if (pasteTime >= timeForNextUpgrade && timeForNextUpgrade > 4)
            {
                countDifficultyUpgrade++;
                pasteTime = 0;

                spawnInterval -= 0.1f;

                CancelInvoke("SpawnFireball");
                InvokeRepeating("SpawnFireball", 0f, spawnInterval);

                switch (countDifficultyUpgrade)//diferentes casos para cuand el contador de mejoras dificultad aumente
                {
                    case 5:
                        timeForNextUpgrade = 8;
                        break;
                    case 10:
                        timeForNextUpgrade = 6;
                        break;
                    case 15:
                        timeForNextUpgrade = 4;
                        moreDifficuty = false;
                        break;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)// si las balls salen del trigger se destruyen
    {
        if (other.CompareTag("Fireball"))
        {
            Destroy(other.gameObject);
        }
    }
#endregion
    #region PlayGame
    public void PlayButton() 
    {
        StartCoroutine(PlayGame());
    }
    IEnumerator PlayGame()// Rutina que controla el inicio del juego
    {
        DestroyObjectsAttack(); //Limpia objetos de ataques existentes

        panelMainMenu.SetActive(false);
        dialoguePanel.SetActive(true);

        while (!cantContinue) // Espera hasta que se permita continuar
        {
            yield return null;
        }

        dialoguePanel.SetActive(false);
        RestartValues();// reinicia valores predeterminados del juego

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        player.cantWalk = true;
        playerLive = true;

        gameController.CargarDatos();// Carga los datos del juego desde el archivo de guardado

        for (int i = 0; i < HighScoreText.Length; i++)  // Actualiza el puntaje más alto en los textos correspondientes
        {
            HighScoreText[i].text = highestScore.ToString();
        }
        InvokeRepeating("SpawnFireball", 0f, spawnInterval);// Inicia la generación periódica de Fireball

        yield return new WaitForSeconds(2f);

        cantContinue = false;
        moreDifficuty = true;
        CantStart = true;
    }
#endregion
    #region PlayerDead
    public void PlayerDead()
    {
        if (playerDeadBool)
            {
                StartCoroutine(PlayerDeadRoutine());
            }
    }
    IEnumerator PlayerDeadRoutine()// Rutina que maneja la muerte del jugador
    {   
        if (objetoActivoAnterior != null)// Reposiciona el objeto activo anterior si existe
        {
            objetoActivoAnterior.transform.position = new Vector2(20,20);
        }

        // Desactiva elementos y detiene la generación
        moreDifficuty = false;
        playerLive = false;
        CantStart = false;

        // Reproduce sonido y realiza acciones post muerte
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.loop = false;
        audioSource.Play();

        CancelInvoke("SpawnFireball");

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        player.cantWalk = false;

        //Espera antes de mostrar el menú de muerte y destruye objetos de ataques
        yield return new WaitForSeconds(1f);

        panelDeadMenu.SetActive(true);
        DestroyObjectsAttack();

        // Guarda el puntaje si es más alto que el anterior
        if (actualScore > highestScore)
        {
            gameController.GuardadoDatos();
        }

        playerDeadBool = false;

        yield return new WaitForSeconds(3f);
    }
#endregion
    #region Reload Buttom
    public void ReloadButton()
    {
        StartCoroutine(ReloadGame());
    }
    IEnumerator ReloadGame()// Rutina que controla la recarga del juego
    {
        // Carga datos y actualiza puntaje más alto
        gameController.CargarDatos();

        for (int i = 0; i < HighScoreText.Length; i++)
        {
            HighScoreText[i].text = highestScore.ToString();
        }
        yield return new WaitForSeconds(0.5f);

        RestartValues();

        yield return new WaitForSeconds(0.5f);

        panelDeadMenu.SetActive(false);

        moreDifficuty = true;
        playerLive = true;
        CantStart = true;

        // Espera antes de iniciar la generación de Fireball
        yield return new WaitForSeconds(0.5f);

        InvokeRepeating("SpawnFireball", 0f, spawnInterval);
    }
    void RestartValues()// Reinicia valores iniciales del juego
    {

        // Reposiciona el objeto de explosión y desactiva la animación de muerte del jugador
        GameObject playerExplosion = GameObject.FindGameObjectWithTag("Explosion");
        playerExplosion.GetComponent<Animator>().SetBool("playerDead",false);

        // Reinicia variables de puntaje, tiempo y dificultad
        actualScore = 0;
        timeForNextUpgrade = 10;
        pasteTime = 0;

        // Habilita el movimiento del jugador y reinicia su posición
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        player.cantWalk = true;
        player.transform.position = new Vector2(0,0);
        playerDeadBool = false;

        // Reinicia variables de generación de FireRay que se cambiaron en la curva de dificultad
        initialFireRayTimerForNext = 10;
        intialTimeForTheNextUpgrade = 10;
        initialCountDifficultyUpgrade = 0;
        initialSpawnInterval = 1.8f;

        fireRayTimerForNext = initialFireRayTimerForNext;
        spawnInterval = initialSpawnInterval;
        timeForNextUpgrade = intialTimeForTheNextUpgrade;
        countDifficultyUpgrade = (int)initialCountDifficultyUpgrade;
    }
#endregion
    #region DestroyObjects
    public void DestroyObjectsAttack()// Destruye objetos de ataques existentes
    {

        // Desactiva el objeto FireRay si está activo
        GameObject fireRayObject = GameObject.FindGameObjectWithTag("FireRay");
        if (fireRayObject != null)
        {
            fireRayObject.SetActive(false);
        }

        // Destruye todos los objetos Fireball presentes en la escena
        GameObject[] fireBalls = GameObject.FindGameObjectsWithTag("Fireball");
        foreach (GameObject fireBall in fireBalls)
        {
            Destroy(fireBall);
        }
        }
    #endregion
    #region SpawnPowerUp
    void PowerUpSpawn()// Genera un PowerUp en una posición aleatoria
    {
        float axis = Random.Range(0f, 1f);
        float spawnX, spawnY;

        // Lógica para determinar la posición de generación del PowerUp
        if (axis < 0.5f)
        {
            spawnX = Random.Range(minX, maxX);
            float side = Random.Range(0f, 1f);
            spawnY = (side < 0.5f) ? mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - offset : mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + offset;
        }
        else
        {
            spawnY = Random.Range(minY, maxY);
            float side = Random.Range(0f, 1f);
            spawnX = (side < 0.5f) ? mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - offset : mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + offset;
        }

        // Instancia el PowerUp y le aplica una velocidad en dirección al centro
        Instantiate(powerUpPrefab, new Vector2(spawnX, spawnY), Quaternion.identity).
        GetComponent<Rigidbody2D>().velocity =
        ((Vector2)transform.position - new Vector2(spawnX,spawnY)).normalized * fireBallSpeed;
    }
    #endregion
}