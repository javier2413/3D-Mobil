using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPersiana : MonoBehaviour
{
    public bool estaAbierta = false;
    private GameDirector director;

    [Header("Configuración de Movimiento")]
    public Vector3 posicionCerrada;
    public Vector3 posicionAbierta;
    public float velocidadAnimacion = 5f;

    void Start()
    {
        director = FindObjectOfType<GameDirector>();


        posicionCerrada = transform.localPosition;

        if (posicionAbierta == Vector3.zero)
        {
            posicionAbierta = posicionCerrada + new Vector3(0, 2f, 0);
        }
    }

    void Update()
    {

        Vector3 objetivo = estaAbierta ? posicionAbierta : posicionCerrada;
        transform.localPosition = Vector3.Lerp(transform.localPosition, objetivo, Time.deltaTime * velocidadAnimacion);
    }

    public void AbrirPersiana()
    {
        if (!estaAbierta)
        {
            estaAbierta = true;
            director.activePersianas++;
            Debug.Log("Persiana abierta. Total: " + director.activePersianas);
        }
    }

    public void CerrarPersiana()
    {
        if (estaAbierta)
        {
            estaAbierta = false;
            director.activePersianas--;
            Debug.Log("Persiana cerrada. Total: " + director.activePersianas);
        }
    }
}
