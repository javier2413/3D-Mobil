using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoSonoro : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    public float tiempoMaximo = 5.0f;
    private float contador = 0f;
    private bool jugadorEnRango = false;

    [Header("Referencias")]
    private AudioSource fuenteAudio;
    private SphereCollider colliderRango;
    private Transform transformJugador;
    private CinemachineImpulseSource fuenteImpulso;

    [Header("Configuración del Temblor")]
    [Tooltip("Intensidad base del temblor")]
    public float fuerzaImpulsoBase = 0.5f;

    void Start()
    {
        // Obtener componentes del mismo objeto
        fuenteAudio = GetComponent<AudioSource>();
        colliderRango = GetComponent<SphereCollider>();
        fuenteImpulso = GetComponent<CinemachineImpulseSource>();

        // Buscar al jugador
        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null) transformJugador = jugadorGO.transform;

        // Verificaciones de seguridad
        if (fuenteAudio == null) Debug.LogError("¡Falta AudioSource en " + gameObject.name + "!");
        if (colliderRango == null) Debug.LogError("¡Falta SphereCollider en " + gameObject.name + "!");
        if (fuenteImpulso == null) Debug.LogError("¡Falta CinemachineImpulseSource en " + gameObject.name + "!");
    }

    void Update()
    {
        if (jugadorEnRango && transformJugador != null)
        {
            contador += Time.deltaTime;

            // 1. Lógica del Sonido (Pitch sube con el tiempo)
            if (fuenteAudio != null)
            {
                fuenteAudio.pitch = 1f + (contador / tiempoMaximo);
            }

            // 2. Lógica del Temblor (Se ejecuta cada frame mientras esté en rango)
            GenerarTemblorProporcional();

            // 3. Lógica de Muerte
            if (contador >= tiempoMaximo)
            {
                MatarJugador();
            }
        }
    }

    void GenerarTemblorProporcional()
    {
        if (fuenteImpulso == null) return;

        // Calculamos distancia para que vibre más fuerte mientras más cerca estés
        float distancia = Vector3.Distance(transform.position, transformJugador.position);
        float radio = colliderRango.radius;

        // 0 en el borde, 1 justo encima del enemigo
        float intensidad = 1f - Mathf.Clamp01(distancia / radio);

        // Disparamos el impulso de Cinemachine
        // El vector determina la dirección aleatoria del 'sacudidón'
        Vector3 direccionAleatoria = Random.insideUnitSphere * intensidad * fuerzaImpulsoBase;
        fuenteImpulso.GenerateImpulse(direccionAleatoria);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            if (fuenteAudio != null && !fuenteAudio.isPlaying) fuenteAudio.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetearEstado();
        }
    }

    void ResetearEstado()
    {
        jugadorEnRango = false;
        contador = 0f;
        if (fuenteAudio != null)
        {
            fuenteAudio.pitch = 1f;
            // Opcional: fuenteAudio.Stop(); si quieres que el sonido pare al salir
        }
    }

    void MatarJugador()
    {
        Debug.Log("GAME OVER: El enemigo sonoro te eliminó");
        // Aquí puedes poner: SceneManager.LoadScene("NombreDeTuEscenaGameOver");
    }
}
