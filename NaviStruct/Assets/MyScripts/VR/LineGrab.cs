using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LineGrab : MonoBehaviour
{
    private LineRenderer lineRenderer = null;   
    private ObjectInteract interactable;
    public HoverHighlight enableHighlight;

    private Vector3[] positions;
    private Vector3 lastHandPos;
    private Quaternion lastHandRot;

    private bool grabbed;
    private bool isHover = false;

    public GameObject dot;

    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction = null;
    public SteamVR_Action_Vector2 touchPadAction = null;
    public SteamVR_Behaviour_Pose trackedObjRight;
    public SteamVR_Behaviour_Pose trackedObjLeft;    

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
            
            interactable.SetKinematic(true);
            interactable.SetMoveScale(transform.position);       
           
            lastHandPos = curHandPos;
            lastHandRot = curHandRot;           

            DisplayLine(false, transform.position);
        }

        else if (clickAction.GetState(targetSource))
        {
            if(isHover)
                enableHighlight.EnableHighlight(true);

            interactable.Move(curHandPos, lastHandPos, curHandRot, lastHandRot);            

            if (touchPadAction.GetAxis(trackedObjRight.inputSource).y > 0)            
                interactable.ScaleUp();            

            else if (touchPadAction.GetAxis(trackedObjRight.inputSource).y < 0)
                interactable.ScaleDown();            

            if (touchPadAction.GetAxis(trackedObjLeft.inputSource).x > 0)            
                interactable.Rotate(true);            

            else if(touchPadAction.GetAxis(trackedObjLeft.inputSource).x < 0)
                interactable.Rotate(false);
        }

        else if (clickAction.GetStateUp(targetSource))
        {
            grabbed = false;            
            interactable.SetKinematic(false);
        }

        lastHandPos = curHandPos;
        lastHandRot = curHandRot;
    }

    private ObjectInteract RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);        

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.gameObject.GetComponent<ObjectInteract>() != null)
            {
                isHover = true;

                enableHighlight.EnableHighlight(true);
                ObjectInteract grabItem = hit.collider.gameObject.GetComponent<ObjectInteract>();
                return grabItem;
            }            

            DisplayLine(true, hit.point);           

            return null;                     
        }

        else
        {
            isHover = false;

            DisplayLine(false, transform.position);            
            enableHighlight.EnableHighlight(false);
            
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
