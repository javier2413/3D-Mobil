using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPersianas : MonoBehaviour
{
    [Header("Configuración")]
    public List<ControlPersiana> todasLasPersianas; 
    public float tiempoEntreAperturas = 10f; 
    private float timer;

    private GameDirector director;

    void Start()
    {
        director = FindObjectOfType<GameDirector>();
        timer = tiempoEntreAperturas;
    }

    void Update()
    {
        if (director.juegoTerminado) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            AbrirUnaAlAzar();
            timer = tiempoEntreAperturas;
        }
    }

    void AbrirUnaAlAzar()
    {
        List<ControlPersiana> cerradas = todasLasPersianas.FindAll(p => !p.estaAbierta);

        if (cerradas.Count > 0)
        {
            int indiceAleatorio = Random.Range(0, cerradas.Count);
            cerradas[indiceAleatorio].AbrirPersiana();
        }
    }
}
