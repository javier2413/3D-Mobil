using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadioTrampa : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    public float tiempoParaMorir = 10f;
    public float tiempoEntreActivaciones = 35f;
    public float distanciaInteraccion = 3f;

    [Header("Referencias")]
    public AudioSource sonidoAlarma;
    public Image barraUI;
    public GameObject contenedorUI;

    private GameDirector director;
    private Transform jugador;
    private float contadorMuerte = 0f;
    private float timerReactivacion = 0f;
    private bool estaActiva = false;
    private bool esperandoParaRepetir = false;
    private bool yaSeActivoHoy = false;

    void Start()
    {
        director = FindObjectOfType<GameDirector>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;

        ResetRadioTotal();
    }

    void Update()
    {
        if (director == null || director.juegoTerminado) return;

        if (director.horaActual == 2 && !yaSeActivoHoy && !estaActiva)
        {
            yaSeActivoHoy = true;
            ActivarRadio();
        }

        if (esperandoParaRepetir && !estaActiva && director.horaActual < 3)
        {
            timerReactivacion += Time.deltaTime;
            if (timerReactivacion >= tiempoEntreActivaciones)
            {
                ActivarRadio();
            }
        }

        if (director.horaActual >= 3)
        {
            if (estaActiva || esperandoParaRepetir) ResetRadioTotal();
            return;
        }

        if (estaActiva)
        {
            contadorMuerte += Time.deltaTime;
            if (barraUI) barraUI.fillAmount = contadorMuerte / tiempoParaMorir;

            if (contadorMuerte >= tiempoParaMorir)
            {
                director.GameOver("La radio atrajo a la muerte");
                estaActiva = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                float distancia = Vector3.Distance(transform.position, jugador.position);
                if (distancia <= distanciaInteraccion)
                {
                    DesactivarRadio();
                }
            }
        }
    }

    void ActivarRadio()
    {
        estaActiva = true;
        esperandoParaRepetir = false;
        contadorMuerte = 0f;
        timerReactivacion = 0f;
        if (sonidoAlarma) sonidoAlarma.Play();
        if (contenedorUI) contenedorUI.SetActive(true);
        Debug.Log("RADIO: Activada a las " + director.horaActual + " AM");
    }

    void DesactivarRadio()
    {
        estaActiva = false;
        esperandoParaRepetir = true;
        timerReactivacion = 0f;
        if (sonidoAlarma) sonidoAlarma.Stop();
        if (contenedorUI) contenedorUI.SetActive(false);
        Debug.Log("RADIO: Apagada. Reiniciando ciclo de 35s.");
    }

    void ResetRadioTotal()
    {
        estaActiva = false;
        esperandoParaRepetir = false;
        if (sonidoAlarma) sonidoAlarma.Stop();
        if (contenedorUI) contenedorUI.SetActive(false);
    }
}
