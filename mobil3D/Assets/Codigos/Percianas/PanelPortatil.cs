using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPortatil : MonoBehaviour
{
    public GameObject objetoTablet;
    public List<ControlPersiana> persianas;
    public List<Image> lucesBotones;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoAlerta;

    private int conteoAnterior = 0;
    private bool tabletActiva = false;

    void Update()
    {
        int conteoActual = 0;
        foreach (var p in persianas)
        {
            if (p.estaAbierta) conteoActual++;
        }

        if (conteoActual > conteoAnterior)
        {
            if (audioSource && sonidoAlerta)
            {
                audioSource.PlayOneShot(sonidoAlerta);
            }
        }

        conteoAnterior = conteoActual;

        if (!tabletActiva) return;

        for (int i = 0; i < persianas.Count; i++)
        {
            lucesBotones[i].color = persianas[i].estaAbierta ? Color.red : Color.green;
        }
    }

    public void AlternarTablet()
    {
        tabletActiva = !tabletActiva;
        objetoTablet.SetActive(tabletActiva);
    }
}
