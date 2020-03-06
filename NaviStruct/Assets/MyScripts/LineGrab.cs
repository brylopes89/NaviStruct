using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LineGrab : MonoBehaviour
{
    private LineRenderer lineRenderer = null;   
    private ObjectInteract interactable;
    private bool grabbed;

    private Vector3[] positions;
    private Vector3 lastHandPos;
    private Quaternion lastHandRot;

    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction = null;
    public SteamVR_Action_Vector2 touchPadAction = null;

    public GameObject dot;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbed)
        {
            interactable = RaycastForInteractable();
            if (!interactable) return;
        }
      
        Vector3 curHandPos = transform.position;        
        Quaternion curHandRot = transform.rotation;

        if (clickAction.GetStateDown(targetSource))
        {
            grabbed = true;

            interactable.Grab(true);
            interactable.SetMoveScale(transform.position);       
           
            lastHandPos = curHandPos;
            lastHandRot = curHandRot;           

            DisplayLine(false, transform.position);
        }

        else if (clickAction.GetState(targetSource))
        {
            interactable.Move(curHandPos, lastHandPos, curHandRot, lastHandRot);           

            if (touchPadAction.GetAxis(targetSource).y > 0)
            {
                interactable.ScaleUp();
            }

            else if (touchPadAction.GetAxis(targetSource).y < 0)
            {
                interactable.ScaleDown();
            }
        }

        else if (clickAction.GetStateUp(targetSource))
        {
            grabbed = false;
            interactable.Grab(false);            
        }

        lastHandPos = curHandPos;
        lastHandRot = curHandRot;
    }

    private ObjectInteract RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<ObjectInteract>() != null)
        {
            ObjectInteract grabItem = hit.collider.gameObject.GetComponent<ObjectInteract>();
            DisplayLine(true, hit.point);

            return grabItem;
        }

        else
        {
            DisplayLine(false, transform.position);
            return null;
        }            
    }

    private void DisplayLine(bool display, Vector3 endpoint)
    {
        lineRenderer.enabled = display;
        positions[0] = transform.position;
        positions[1] = endpoint;
        lineRenderer.SetPositions(positions);
        dot.transform.position = endpoint;
    }
}
