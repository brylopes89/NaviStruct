using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectSelector : MonoBehaviour
{    
    public SteamVR_Behaviour_Pose trackedObj;
    public SteamVR_Action_Boolean trackPadAction;
    public GameObject pointer;

    private Collider col;
    private Transform currentTransform;
    bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();        
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        if (trackPadAction.GetStateDown(trackedObj.inputSource))
        {
            dragging = false;
            Ray ray = new Ray(trackedObj.transform.position, trackedObj.transform.forward);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                dragging = true;
            }
        }

        if (trackPadAction.GetStateUp(trackedObj.inputSource))
        {
            dragging = false;
            transform.parent = null;
        }  

        if (dragging && trackPadAction.GetState(trackedObj.inputSource))
        {
            Ray ray = new Ray(trackedObj.transform.position, trackedObj.transform.forward);
            RaycastHit hit;

            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                transform.parent = pointer.transform;
            }
        }
    }
}
