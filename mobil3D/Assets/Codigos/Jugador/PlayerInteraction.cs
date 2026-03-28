using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float distanciaInteraccion = 5f;
    public LayerMask capaInteractivas; 

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanciaInteraccion, capaInteractivas, QueryTriggerInteraction.Collide))
            {
                ControlPersiana p = hit.transform.GetComponent<ControlPersiana>();

                if (p != null)
                {
                    p.CerrarPersiana();
                }
            }
        }
    }
}
