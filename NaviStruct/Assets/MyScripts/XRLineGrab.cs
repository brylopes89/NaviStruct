using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRLineGrab : MonoBehaviour
{    
    //private LineRenderer lineRenderer = null;    
    private XRGrabInteractable interactable;

    private Vector3[] positions;
    private Vector3 lastHandPos;
    private Vector3 curHandPos;

    private Quaternion curHandRot;
    private Quaternion lastHandRot;

    private bool grabbed;   

    // Start is called before the first frame update
    void Start()
    {
        //lineRenderer = GetComponent<LineRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {        
        interactable = RaycastForInteractable();
        if (!interactable) return;

        //interactable.attachTransform = transform;

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

    /*private void DisplayLine(bool display, Vector3 endpoint)
    {
        lineRenderer.enabled = display;
        positions[0] = transform.position;
        positions[1] = endpoint;
        lineRenderer.SetPositions(positions);
        dot.transform.position = endpoint;
    }*/
}
