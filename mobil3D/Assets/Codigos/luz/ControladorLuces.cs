using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorLuces : MonoBehaviour
{
    [Header("Configuración de Luces")]
    public List<Light> lucesMercado;
    public float tiempoEntreApagado = 30f;
    public float tiempoMuerteTrasQuintaLuz = 15f;

    [Header("Configuración de Parpadeo Ambiental")]
    [Range(0, 1)] public float probabilidadParpadeo = 0.15f;
    public float intervaloChequeoParpadeo = 4f;

    [Header("Referencias Jugador")]
    public Light luzLinterna;
    private GameDirector director;
    private bool todasApagadas = false;
    private float timerApagado = 0f;
    private int lucesEncendidasContador;

    [HideInInspector] public bool esPeligrosoReparar = false;

    [Header("Sonidos Globales (2D)")]
    public AudioSource fuenteAudioGlobal;
    public AudioClip sonidoApagonTotal;
    public AudioClip sonidoGeneradorReparado;

    [Header("Sonidos Individuales (3D)")]
    public AudioClip sonidoFalloLuz;

    void Start()
    {
        director = FindObjectOfType<GameDirector>();
        lucesEncendidasContador = lucesMercado.Count;
        if (fuenteAudioGlobal == null) fuenteAudioGlobal = GetComponent<AudioSource>();

        foreach (Light l in lucesMercado)
        {
            AudioSource zumbido = l.GetComponent<AudioSource>();
            if (zumbido != null)
            {
                zumbido.loop = true;
                zumbido.playOnAwake = true;
                zumbido.spatialBlend = 1.0f;
                zumbido.Play();
            }
        }

        StartCoroutine(CicloParpadeoAleatorio());
    }

    void Update()
    {
       
        if (director == null || director.juegoTerminado) return;

        
        if (director.horaActual < 3 && director.horaActual != 12) return;
       

        if (director.horaActual >= 3)
        {
            if (lucesEncendidasContador > 0)
            {
                timerApagado += Time.deltaTime;
                if (timerApagado >= tiempoEntreApagado)
                {
                    ApagarSiguienteLuz();
                    timerApagado = 0f;
                }
            }
            else if (!todasApagadas)
            {
                todasApagadas = true;
                StartCoroutine(SecuenciaFinalOscuridad());
            }
        }
    }

    IEnumerator CicloParpadeoAleatorio()
    {
        while (true)
        {
            if (director != null && director.horaActual >= 3 && !todasApagadas && lucesEncendidasContador > 0)
            {
                if (Random.value < probabilidadParpadeo)
                {
                    StartCoroutine(HacerParpadearLuzAlAzar());
                }
            }
            yield return new WaitForSeconds(intervaloChequeoParpadeo);
        }
    }

    IEnumerator HacerParpadearLuzAlAzar()
    {
        int indiceLuz = Random.Range(0, lucesEncendidasContador);
        Light luzElegida = lucesMercado[indiceLuz];

        if (luzElegida != null && luzElegida.enabled)
        {
            int repeticiones = Random.Range(2, 5);
            for (int i = 0; i < repeticiones; i++)
            {
                luzElegida.enabled = false;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
                luzElegida.enabled = true;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
        }
    }

    void ApagarSiguienteLuz()
    {
        if (lucesEncendidasContador > 0)
        {
            lucesEncendidasContador--;
            Light luzActual = lucesMercado[lucesEncendidasContador];
            luzActual.enabled = false;

            AudioSource audioDeLaLampara = luzActual.GetComponent<AudioSource>();
            if (audioDeLaLampara != null)
            {
                audioDeLaLampara.Stop();
                if (sonidoFalloLuz != null)
                    audioDeLaLampara.PlayOneShot(sonidoFalloLuz);
            }
        }
    }

    IEnumerator SecuenciaFinalOscuridad()
    {
        if (fuenteAudioGlobal != null && sonidoApagonTotal != null)
            fuenteAudioGlobal.PlayOneShot(sonidoApagonTotal);

        float tiempoRestante = tiempoMuerteTrasQuintaLuz;

        while (tiempoRestante > 0)
        {
            if (todasApagadas == false) yield break;
            tiempoRestante -= Time.deltaTime;
            if (luzLinterna != null) luzLinterna.enabled = Random.value > 0.2f;
            yield return null;
        }

        esPeligrosoReparar = true;
        if (luzLinterna != null) luzLinterna.enabled = false;
    }

    public void RepararGenerador()
    {
        StopAllCoroutines();
        todasApagadas = false;
        esPeligrosoReparar = false;
        lucesEncendidasContador = lucesMercado.Count;
        timerApagado = 0f;

        if (fuenteAudioGlobal != null && sonidoGeneradorReparado != null)
            fuenteAudioGlobal.PlayOneShot(sonidoGeneradorReparado);

        foreach (Light l in lucesMercado)
        {
            l.enabled = true;
            AudioSource zumbido = l.GetComponent<AudioSource>();
            if (zumbido != null) zumbido.Play();
        }

        if (luzLinterna != null) luzLinterna.enabled = true;
        StartCoroutine(CicloParpadeoAleatorio());
    }
}
