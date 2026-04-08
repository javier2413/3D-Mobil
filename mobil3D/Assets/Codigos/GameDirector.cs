using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameDirector : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public float tiempoRealSegundos = 360f;
    private float tiempoTranscurrido = 0f;
    public int horaActual = 12;
    public TextMeshProUGUI textoReloj;

    [Header("Enemigos por Hora")]
    public GameObject enemigoCadenas;
    private bool enemigoCadenasActivado = false;

    private ControladorLuces sistemaLuces;
    private bool sistemaLucesIniciado = false;

    [Header("Estado de la Partida")]
    public int activePersianas = 0;
    public int limitePersianas = 5;
    public bool juegoTerminado = false;

    [Header("Referencias Cinemachine")]
    public CinemachineBrain brain;
    public GameObject objetoJugador;

    [Header("Efecto de Muerte")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;
    public Image imagenNegra;

    [Tooltip("Tiempo en segundos que dura la caída")]
    [Range(1.0f, 10.0f)]
    public float duracionCaida = 3f;

    void Start()
    {
        sistemaLuces = FindObjectOfType<ControladorLuces>();
    }

    void Update()
    {
        if (juegoTerminado) return;

        tiempoTranscurrido += Time.deltaTime;

      
        float progresoHora = tiempoTranscurrido / (tiempoRealSegundos / 6f);
        int nuevaHora = 12 + Mathf.FloorToInt(progresoHora);

        if (nuevaHora > 12) { horaActual = nuevaHora - 12; }
        else { horaActual = 12; }

        if (textoReloj != null) { textoReloj.text = horaActual.ToString() + " AM"; }

        CheckEnemiesByHour();

        if (horaActual == 6)
        {
            WinGame();
        }

        if (activePersianas >= limitePersianas)
        {
            GameOver("Persianas bloqueadas");
        }
    }

    void CheckEnemiesByHour()
    {
        if (horaActual == 1 && !enemigoCadenasActivado)
        {
            if (enemigoCadenas != null)
            {
                enemigoCadenasActivado = true;
                enemigoCadenas.SetActive(true);

                EnemigoSonoro scriptEnemigo = enemigoCadenas.GetComponent<EnemigoSonoro>();
                if (scriptEnemigo != null)
                {
                    scriptEnemigo.Invoke("TeletransportarEnemigo", 0.1f);
                }
                Debug.Log("SISTEMA: 1 AM - El enemigo de las cadenas acecha.");
            }
        }

      
        if (horaActual == 3 && !sistemaLucesIniciado)
        {
            sistemaLucesIniciado = true;
            Debug.Log("SISTEMA: 3 AM - El sistema eléctrico del mercado está fallando.");
        }
    }


    public void WinGame()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        SceneManager.LoadScene("WinScene");
    }

    public void GameOver(string razon)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        StartCoroutine(SecuenciaMuerteCaida());
    }

   
    IEnumerator SecuenciaMuerteCaida()
    {
       
        if (brain != null) brain.enabled = false;
      
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("LoseScene");
    }
}
