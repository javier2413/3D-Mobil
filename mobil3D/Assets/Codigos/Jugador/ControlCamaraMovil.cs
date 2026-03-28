using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamaraMovil : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float intensidadCaminar = 0.5f;
    public float frecuenciaCaminar = 1.2f;
    public float suavidadTransicion = 5f;

    private CinemachineVirtualCamera _vCam;
    private CinemachineBasicMultiChannelPerlin _ruido;
    private StarterAssetsInputs _input;

    void Start()
    {
       
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _ruido = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

      
        if (_vCam.Follow != null)
        {
            _input = _vCam.Follow.GetComponentInParent<StarterAssetsInputs>();
        }
    }

    void Update()
    {
        if (_ruido == null || _input == null) return;

       
        if (_input.move.magnitude > 0.1f)
        {
            
            _ruido.m_AmplitudeGain = Mathf.Lerp(_ruido.m_AmplitudeGain, intensidadCaminar, Time.deltaTime * suavidadTransicion);
            _ruido.m_FrequencyGain = frecuenciaCaminar;
        }
        else
        {
           
            _ruido.m_AmplitudeGain = Mathf.Lerp(_ruido.m_AmplitudeGain, 0f, Time.deltaTime * suavidadTransicion);
        }
    }
}
