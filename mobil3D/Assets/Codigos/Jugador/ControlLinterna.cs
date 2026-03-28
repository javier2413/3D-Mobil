using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLinterna : MonoBehaviour
{
    public GameObject luzLinterna; 
    private bool estaEncendida = false; 

    private StarterAssetsInputs _inputs;

    void Start()
    {
       
        _inputs = GetComponent<StarterAssetsInputs>();

        
        if (luzLinterna != null) luzLinterna.SetActive(estaEncendida);
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.F))
        {
            AlternarLinterna();
        }

        
        if (_inputs != null && _inputs.flashlight)
        {
            AlternarLinterna();

           
            _inputs.flashlight = false;
        }
    }

    public void AlternarLinterna()
    {
        estaEncendida = !estaEncendida;
        if (luzLinterna != null)
        {
            luzLinterna.SetActive(estaEncendida);
        }
    }
}
