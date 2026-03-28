using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    [Header("Efecto de Muerte (Sin Animación)")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;
    public float velocidadCaida = 0.5f;

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

        // Si llegamos al límite, iniciamos la secuencia de caída
        if (activePersianas >= limitePersianas)
        {
            StartCoroutine(SecuenciaMuerteCaida());
        }
    }

    // --- EL TRUCO DE LA CAÍDA ---
    IEnumerator SecuenciaMuerteCaida()
    {
        juegoTerminado = true;

        // 1. Buscamos la cámara principal
        Transform camara = Camera.main.transform;
        Vector3 posInicial = camara.localPosition;
        Quaternion rotInicial = camara.localRotation;

        // 2. Definimos el suelo y la rotación de lado (80 grados)
        Vector3 posSuelo = new Vector3(posInicial.x, 0.2f, posInicial.z);
        Quaternion rotLado = Quaternion.Euler(posInicial.x, posInicial.y, 80f);

        // 3. Sonido de impacto
        if (audioSource != null && sonidoGolpe != null)
        {
            audioSource.PlayOneShot(sonidoGolpe);
        }

        // 4. Movemos la cámara físicamente por código
        float tiempo = 0;
        while (tiempo < velocidadCaida)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / velocidadCaida;

            camara.localPosition = Vector3.Lerp(posInicial, posSuelo, progreso);
            camara.localRotation = Quaternion.Lerp(rotInicial, rotLado, progreso);
            yield return null;
        }

        // 5. Esperamos un segundo en el suelo y cargamos la escena de perder
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("LoseScene");
    }

    void CheckEnemiesByHour()
    {
        // Aquí programaremos las Cadenas a la 1 AM después
    }

    public void WinGame()
    {
        juegoTerminado = true;
        SceneManager.LoadScene("WinScene");
    }

    public void GameOver(string razon)
    {
        // Esta función la dejamos por si otros enemigos te matan de golpe
        juegoTerminado = true;
        SceneManager.LoadScene("LoseScene");
    }
}
