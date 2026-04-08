using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemigoSonoro : MonoBehaviour
{
    [Header("Configuración de Acecho")]
    public float radioAparicion = 15f;
    public float tiempoVisible = 20f; 
    public float esperaMinima = 5f;  
    public float esperaMaxima = 10f; 

    private float timerAcecho = 0f;
    private bool estaEsperando = false;

    [Header("Configuración de Muerte")]
    public float tiempoMaximoAtaque = 5.0f;
    private float contadorAtaque = 0f;
    private bool jugadorEnRango = false;
    private bool estaMuriendo = false;

    [Header("Referencias UI Muerte")]
    public GameObject objetoPantallaNegra;
    public float duracionFundidoNegro = 2.5f;

    [Header("Sonidos")]
    public AudioSource fuenteAudioCadenas;
    public AudioSource audioSourceJugador;
    public AudioClip sonidoGritosAhorcado;
    public AudioClip sonidoCadenasApretando;

    [Header("Referencias Generales")]
    private Transform transformJugador;
    private CinemachineImpulseSource fuenteImpulso;
    private GameDirector director;

    [Header("Configuración del Temblor")]
    public float fuerzaImpulsoBase = 0.5f;
    public float intervaloTemblor = 0.1f;
    private float timerTemblor = 0f;

    private void OnEnable()
    {
        if (fuenteAudioCadenas != null)
        {
            fuenteAudioCadenas.pitch = 1f;
        }

        StartCoroutine(CicloDeAparicion());
    }

    void Start()
    {
        fuenteImpulso = GetComponent<CinemachineImpulseSource>();
        director = FindObjectOfType<GameDirector>();
        if (fuenteAudioCadenas == null) fuenteAudioCadenas = GetComponent<AudioSource>();

        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null) transformJugador = jugadorGO.transform;

        if (objetoPantallaNegra == null) objetoPantallaNegra = GameObject.FindWithTag("Muerte");
    }

    void Update()
    {
        if (estaMuriendo || estaEsperando || (director != null && director.juegoTerminado)) return;

        if (jugadorEnRango)
        {
            ActualizarAtaque();
        }
        else
        {
            if (fuenteAudioCadenas != null) fuenteAudioCadenas.pitch = 1f;
            contadorAtaque = 0f;
        }
    }

    IEnumerator CicloDeAparicion()
    {
        while (!estaMuriendo)
        {
            estaEsperando = true;
            if (fuenteAudioCadenas != null) fuenteAudioCadenas.Stop();

            transform.position = new Vector3(0, -100, 0);

            float tiempoEspera = Random.Range(esperaMinima, esperaMaxima);
            yield return new WaitForSeconds(tiempoEspera);

            TeletransportarEnemigo();
            estaEsperando = false;
            if (fuenteAudioCadenas != null) fuenteAudioCadenas.Play();

            yield return new WaitForSeconds(tiempoVisible);
        }
    }

    public void TeletransportarEnemigo()
    {
        if (transformJugador == null) return;

        Vector2 puntoAleatorio = Random.insideUnitCircle.normalized * radioAparicion;
        Vector3 nuevaPos = new Vector3(transformJugador.position.x + puntoAleatorio.x, transformJugador.position.y, transformJugador.position.z + puntoAleatorio.y);

        transform.position = nuevaPos;
        Debug.Log("Las cadenas han reaparecido en una nueva posición.");
    }

    void ActualizarAtaque()
    {
        contadorAtaque += Time.deltaTime;
        if (fuenteAudioCadenas != null)
            fuenteAudioCadenas.pitch = 1f + (contadorAtaque / tiempoMaximoAtaque);

        timerTemblor += Time.deltaTime;
        if (timerTemblor >= intervaloTemblor)
        {
            GenerarTemblorProporcional();
            timerTemblor = 0f;
        }

        if (contadorAtaque >= tiempoMaximoAtaque)
        {
            estaMuriendo = true;
            StopAllCoroutines();
            StartCoroutine(SecuenciaMuerteAhorcamiento());
        }
    }

    void GenerarTemblorProporcional()
    {
        if (fuenteImpulso == null || transformJugador == null) return;
        float factorMuerte = contadorAtaque / tiempoMaximoAtaque;
        Vector3 direccion = Random.insideUnitSphere * fuerzaImpulsoBase * factorMuerte;
        fuenteImpulso.GenerateImpulse(direccion);
    }

    IEnumerator SecuenciaMuerteAhorcamiento()
    {
        if (director != null) director.juegoTerminado = true;

        if (transformJugador.GetComponent<CharacterController>())
            transformJugador.GetComponent<CharacterController>().enabled = false;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.enabled = false;

        Transform camaraTransform = Camera.main.transform;
        Vector3 posInicial = camaraTransform.position;
        Quaternion rotInicial = camaraTransform.rotation;

        Vector3 posLevantado = posInicial + Vector3.up * 1.5f;
        Vector3 posSuelo = new Vector3(posInicial.x, 0.3f, posInicial.z);
        Quaternion rotLado = rotInicial * Quaternion.Euler(0, 0, 70f);

        if (audioSourceJugador != null)
        {
            if (fuenteAudioCadenas != null) fuenteAudioCadenas.Stop();
            audioSourceJugador.PlayOneShot(sonidoCadenasApretando);
        }

        float tiempo = 0;
        float duracionLevantar = 2.0f;
        Image img = objetoPantallaNegra != null ? objetoPantallaNegra.GetComponent<Image>() : null;

        while (tiempo < duracionLevantar)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionLevantar;
            float suavizado = t * t * (3f - 2f * t);

            camaraTransform.position = Vector3.Lerp(posInicial, posLevantado, suavizado);

            if (img != null) img.color = new Color(0, 0, 0, t * 0.8f);

            yield return null;
        }

      
        if (audioSourceJugador != null) audioSourceJugador.PlayOneShot(sonidoGritosAhorcado);

        tiempo = 0;
        float duracionCaidaRapida = 0.5f;
        Vector3 posAntesDeCaer = camaraTransform.position;

        while (tiempo < duracionCaidaRapida)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionCaidaRapida;

           
            camaraTransform.position = Vector3.Lerp(posAntesDeCaer, posSuelo, t * t);
            camaraTransform.rotation = Quaternion.Lerp(rotInicial, rotLado, t);

            if (img != null) img.color = new Color(0, 0, 0, 0.8f + (t * 0.2f));

            yield return null;
        }

        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("LoseScene");
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorEnRango = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorEnRango = false; }
}