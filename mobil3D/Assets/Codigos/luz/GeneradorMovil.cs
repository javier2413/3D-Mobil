using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneradorMovil : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public float distanciaInteraccion = 4f;
    public float tiempoNecesario = 3f;
    private float tiempoActual = 0f;

    [Header("UI (Opcional)")]
    public Image barraProgreso;

    private ControladorLuces sistemaLuces;
    private MuerteElectricidad scriptMuerte; // Referencia al nuevo script de muerte
    private Transform jugador;

    void Start()
    {
        sistemaLuces = FindObjectOfType<ControladorLuces>();
        scriptMuerte = GetComponent<MuerteElectricidad>(); // Obtenemos el script de muerte que está en este mismo objeto

        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null) jugador = jugadorGO.transform;

        if (barraProgreso != null) barraProgreso.fillAmount = 0;
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Solo si el jugador está cerca detectamos el toque
        if (distancia <= distanciaInteraccion)
        {
            if (Input.GetMouseButton(0))
            {
                EmpezarAReparar();
            }
            else
            {
                DetenerReparacion();
            }
        }
        else
        {
            DetenerReparacion();
        }
    }

    void EmpezarAReparar()
    {
        tiempoActual += Time.deltaTime;

        if (barraProgreso != null)
            barraProgreso.fillAmount = tiempoActual / tiempoNecesario;

        if (tiempoActual >= tiempoNecesario)
        {
            if (sistemaLuces.esPeligrosoReparar)
            {
                GetComponent<MuerteElectricidad>().EjecutarMuerte();
            }
            else
            {
                sistemaLuces.RepararGenerador();
            }

            tiempoActual = 0;
            if (barraProgreso != null) barraProgreso.fillAmount = 0;
        }
    }

    void DetenerReparacion()
    {
        tiempoActual = 0;
        if (barraProgreso != null) barraProgreso.fillAmount = 0;
    }
}
