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

    void Update()
    {
        if (juegoTerminado) return;

        tiempoTranscurrido += Time.deltaTime;
        int nuevaHora = 12 + Mathf.FloorToInt(tiempoTranscurrido / 60f);
        if (nuevaHora > 12) { horaActual = nuevaHora - 12; }
        else { horaActual = 12; }

        if (textoReloj != null) { textoReloj.text = horaActual.ToString() + " AM"; }

        CheckEnemiesByHour();

        if (tiempoTranscurrido >= tiempoRealSegundos || horaActual == 6)
        {
            WinGame();
        }

        if (activePersianas >= limitePersianas)
        {
            juegoTerminado = true;
            StartCoroutine(SecuenciaMuerteCaida());
        }
    }

    IEnumerator SecuenciaMuerteCaida()
    {
        if (brain != null) brain.enabled = false;
        if (objetoJugador != null) objetoJugador.SetActive(false);

        Transform camaraTransform = Camera.main.transform;
        camaraTransform.SetParent(null);

        Vector3 posInicial = camaraTransform.position;
        Quaternion rotInicial = camaraTransform.rotation;

        Vector3 posSuelo = new Vector3(posInicial.x, 0.3f, posInicial.z);
        Quaternion rotLado = rotInicial * Quaternion.Euler(0, 0, 80f);

        if (audioSource != null && sonidoGolpe != null)
        {
            audioSource.PlayOneShot(sonidoGolpe);
        }

        float tiempo = 0;
        while (tiempo < duracionCaida)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionCaida;
            float suavizado = t * t * (3f - 2f * t);

            camaraTransform.position = Vector3.Lerp(posInicial, posSuelo, suavizado);
            camaraTransform.rotation = Quaternion.Lerp(rotInicial, rotLado, suavizado);

            if (imagenNegra != null)
            {
                Color c = imagenNegra.color;
                c.a = Mathf.Lerp(0, 1, suavizado);
                imagenNegra.color = c;
            }

            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("LoseScene");
    }

    void CheckEnemiesByHour() 
    {

    }
    public void WinGame()
    {
        juegoTerminado = true; SceneManager.LoadScene("WinScene");
    }
    public void GameOver(string razon)
    { 
        juegoTerminado = true; SceneManager.LoadScene("LoseScene");
    }
}
