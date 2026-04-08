using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuerteElectricidad : MonoBehaviour
{
    [Header("UI y Referencias Globales")]
    public Image pantallaNegra;
    public AudioSource audioGlobal;
    public AudioClip sonidoElectrocución;
    private GameDirector director;

    [Header("Efectos Visuales (VFX) en el Generador")]
    public ParticleSystem chispasElectricas;
    public Light luzFlickerElectrico;
    public float intensidadLuzMaxima = 10f; 

    void Start()
    {
        director = FindObjectOfType<GameDirector>();
        if (chispasElectricas != null) chispasElectricas.Stop();
        if (luzFlickerElectrico != null) luzFlickerElectrico.intensity = 0;
    }

    public void EjecutarMuerte()
    {
        StartCoroutine(AnimacionMuerteElectrica());
    }

    IEnumerator AnimacionMuerteElectrica()
    {
        Debug.Log("SISTEMA: Iniciando muerte por electrocución.");

        if (audioGlobal != null && sonidoElectrocución != null)
            audioGlobal.PlayOneShot(sonidoElectrocución);

        if (chispasElectricas != null) chispasElectricas.Play();

        float t = 0;
        float duracionSusto = 0.8f;

        while (t < duracionSusto)
        {
            if (luzFlickerElectrico != null)
                luzFlickerElectrico.intensity = Random.Range(0, intensidadLuzMaxima);

           
            float alphaUI = Random.Range(0.2f, 0.9f);
            Color colorFlicker = new Color(0, Random.Range(0f, 0.4f), Random.Range(0.5f, 1f), alphaUI);
            pantallaNegra.color = colorFlicker;

          
            float tiempoEspera = Random.Range(0.01f, 0.05f);
            yield return new WaitForSeconds(tiempoEspera);
            t += tiempoEspera;
        }

        pantallaNegra.color = Color.black;
        if (luzFlickerElectrico != null) luzFlickerElectrico.intensity = 0;
        if (chispasElectricas != null) chispasElectricas.Stop();

        yield return new WaitForSeconds(1.5f);
        director.GameOver("Electrocutado");
    }
}
