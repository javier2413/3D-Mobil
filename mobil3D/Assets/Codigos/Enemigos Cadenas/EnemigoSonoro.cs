using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemigoSonoro : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    public float tiempoMaximoAtaque = 5.0f;
    private float contadorAtaque = 0f;
    private bool jugadorEnRango = false;
    private bool estaMuriendo = false;

    [Header("Referencias UI Muerte")]
    [Tooltip("Arrastra aquí el objeto de la Jerarquía que tiene la Image negra")]
    public GameObject objetoPantallaNegra;
    public float duracionFundidoNegro = 2.5f;

    [Header("Sonidos de Muerte")]
    public AudioSource audioSourceJugador;
    public AudioClip sonidoGritosAhorcado;
    public AudioClip sonidoCadenasApretando;

    [Header("Referencias Enemigo")]
    private AudioSource fuenteAudioCadenas;
    private SphereCollider colliderRango;
    private Transform transformJugador;
    private CinemachineImpulseSource fuenteImpulso;
    private GameDirector director;

    [Header("Configuración del Temblor")]
    public float fuerzaImpulsoBase = 0.5f;
    public float intervaloTemblor = 0.1f;
    private float timerTemblor = 0f;

    void Start()
    {
        fuenteAudioCadenas = GetComponent<AudioSource>();
        colliderRango = GetComponent<SphereCollider>();
        fuenteImpulso = GetComponent<CinemachineImpulseSource>();

        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null) transformJugador = jugadorGO.transform;

        director = FindObjectOfType<GameDirector>();

        if (objetoPantallaNegra == null)
        {
            objetoPantallaNegra = GameObject.FindWithTag("Muerte");
        }
    }

    void Update()
    {
        if (estaMuriendo || (director != null && director.juegoTerminado)) return;

        if (jugadorEnRango && transformJugador != null)
        {
            contadorAtaque += Time.deltaTime;

            if (fuenteAudioCadenas != null)
            {
                fuenteAudioCadenas.pitch = 1f + (contadorAtaque / tiempoMaximoAtaque);
            }

            timerTemblor += Time.deltaTime;
            if (timerTemblor >= intervaloTemblor)
            {
                GenerarTemblorProporcional();
                timerTemblor = 0f;
            }

            if (contadorAtaque >= tiempoMaximoAtaque)
            {
                estaMuriendo = true;
                StartCoroutine(SecuenciaMuerteAhorcamiento());
            }
        }
    }

    void GenerarTemblorProporcional()
    {
        if (fuenteImpulso == null || estaMuriendo) return;

        float distancia = Vector3.Distance(transform.position, transformJugador.position);
        float radio = colliderRango.radius;
        float intensidad = 1f - Mathf.Clamp01(distancia / radio);
        float factorMuerte = contadorAtaque / tiempoMaximoAtaque;

        Vector3 direccionAleatoria = Random.insideUnitSphere * intensidad * fuerzaImpulsoBase * factorMuerte;
        fuenteImpulso.GenerateImpulse(direccionAleatoria);
    }

    IEnumerator SecuenciaMuerteAhorcamiento()
    {
        if (director != null) director.juegoTerminado = true;

        // Intentar detener el movimiento del jugador
        if (transformJugador != null)
        {
            var controller = transformJugador.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
        }

        // Desactivar el Brain de Cinemachine para congelar la cámara
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.enabled = false;

        // Iniciar sonidos de muerte
        if (audioSourceJugador != null)
        {
            if (fuenteAudioCadenas != null) fuenteAudioCadenas.Stop();
            if (sonidoCadenasApretando != null) audioSourceJugador.PlayOneShot(sonidoCadenasApretando);
            yield return new WaitForSeconds(0.5f);
            if (sonidoGritosAhorcado != null) audioSourceJugador.PlayOneShot(sonidoGritosAhorcado);
        }

        // Fundido a negro (Asfixia)
        float tiempoFundido = 0;
        Color colorMuerte = Color.black;
        colorMuerte.a = 0;

        // Obtenemos el componente Image del GameObject que arrastraste
        Image imgComp = null;
        if (objetoPantallaNegra != null) imgComp = objetoPantallaNegra.GetComponent<Image>();

        while (tiempoFundido < duracionFundidoNegro)
        {
            tiempoFundido += Time.deltaTime;
            float t = tiempoFundido / duracionFundidoNegro;

            if (imgComp != null)
            {
                colorMuerte.a = Mathf.Lerp(0, 1, t);
                imgComp.color = colorMuerte;
            }
            yield return null;
        }

        // Espera final para que se escuchen los últimos sonidos en la oscuridad
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("LoseScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaMuriendo) return;
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            if (fuenteAudioCadenas != null && !fuenteAudioCadenas.isPlaying) fuenteAudioCadenas.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (estaMuriendo) return;
        if (other.CompareTag("Player"))
        {
            ResetearEstado();
        }
    }

    void ResetearEstado()
    {
        jugadorEnRango = false;
        contadorAtaque = 0f;
        if (fuenteAudioCadenas != null) fuenteAudioCadenas.pitch = 1f;
    }
}
