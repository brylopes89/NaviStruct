using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRLineGrab : MonoBehaviour
{    
    //private LineRenderer lineRenderer = null;
    private ObjectInteract interactable;    

    private Vector3[] positions;
    private Vector3 lastHandPos;
    private Vector3 curHandPos;

    private Quaternion curHandRot;
    private Quaternion lastHandRot;

    private bool grabbed;

    public bool setMove;      

    // Start is called before the first frame update
    void Start()
    {
        //lineRenderer = GetComponent<LineRenderer>();
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

        //GetStateDown
        Vector3 curHandPos = transform.position;
        Quaternion curHandRot = transform.rotation;

        if (setMove == true)
        {
            grabbed = true;

            interactable.SetKinematic(true);
            interactable.SetMoveScale(transform.position);
            interactable.Move(curHandPos, lastHandPos, curHandRot, lastHandRot);

            lastHandPos = curHandPos;
            lastHandRot = curHandRot;
        }
        
        if(setMove == false)
        {            
            grabbed = false;
            interactable.SetKinematic(false);
        }

        lastHandPos = curHandPos;
        lastHandRot = curHandRot; 

        //interactable.ScaleUp();

        //interactable.ScaleDown();

        //interactable.Rotate(true);

        //interactable.Rotate(false);
    }

    private void SetMovePos(Vector3 curHandPos, Quaternion curHandRot)
    {        
        grabbed = true;

        interactable.SetKinematic(true);
        interactable.SetMoveScale(transform.position);

        lastHandPos = curHandPos;
        lastHandRot = curHandRot;
    }

    private ObjectInteract RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.GetComponent<ObjectInteract>() != null)
            {                
                ObjectInteract grabItem = hit.collider.gameObject.GetComponent<ObjectInteract>();
                return grabItem;
            }            

            return null;
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
