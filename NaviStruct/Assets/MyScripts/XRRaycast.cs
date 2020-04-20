using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRaycast : MonoBehaviour
{    
    private XRInteractorLineVisual lineVisual;    
    private XRGrabInteractable interactable;

    private LineRenderer lineRenderer = null;
    private Vector3[] positions;
    private void Start()
    {
        lineVisual = GetComponent<XRInteractorLineVisual>();
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector3[2];
    }
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.GetComponent<XRGrabInteractable>())
            {
                XRGrabInteractable grabInteractable = hit.collider.gameObject.GetComponent<XRGrabInteractable>();
                return grabInteractable;
            }

            DisplayLine(true, hit.point);

            return null;           
        }        
        else
        {
            DisplayLine(false, transform.position);
            return null;
        }        
             
    }

    private void DisplayLine(bool display, Vector3 endpoint)
    {
        lineVisual.enabled = display;
        positions[0] = transform.position;
        positions[1] = endpoint;
        lineRenderer.SetPositions(positions);
    }
}
