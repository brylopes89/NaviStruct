using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRaycast : MonoBehaviour
{    
    //private LineRenderer lineRenderer = null;    
    private XRGrabInteractable interactable;

    // Update is called once per frame
    void Update()
    {        
        interactable = RaycastForInteractable();
        if (!interactable) return;

        interactable.attachTransform = transform;

    }

    private XRGrabInteractable RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<XRGrabInteractable>())
        {
                     
            XRGrabInteractable grabInteractable = hit.collider.gameObject.GetComponent<XRGrabInteractable>();
            return grabInteractable;     
        }

        else        
          return null;        
    }
}
