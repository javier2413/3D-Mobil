using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidosAleatoriosMenu : MonoBehaviour
{
    public AudioSource fuenteAudio;
    public List<AudioClip> sonidosAmbiente;

    [Header("Configuración de Tiempo")]
    public float tiempoMinimo = 5f;
    public float tiempoMaximo = 15f;

    void Start()
    {
        if (fuenteAudio == null) fuenteAudio = GetComponent<AudioSource>();

        StartCoroutine(CicloDeSonidos());
    }

    IEnumerator CicloDeSonidos()
    {
        while (true)
        {
            float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(tiempoEspera);

            if (sonidosAmbiente.Count > 0)
            {
                int indiceAleatorio = Random.Range(0, sonidosAmbiente.Count);
                AudioClip clipElegido = sonidosAmbiente[indiceAleatorio];

                fuenteAudio.PlayOneShot(clipElegido);
            }
        }
    }
}
