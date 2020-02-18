using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectSelector : MonoBehaviour
{
    public SteamVR_Behaviour_Pose trackedObjRight;
    public SteamVR_Behaviour_Pose trackedObjLeft;
    public SteamVR_Action_Boolean selectAction;
    public SteamVR_Action_Boolean trackPadAction;    
    public SteamVR_Action_Vector2 moveValue = null;
    public GameObject pointerRight;
    public GameObject pointerLeft;
    
    public float maxSpeed = 1.0f;

    private float startingDistance;
   // private float rotSpeed = 0.5f;
    private bool dragging;
    private Collider col;        

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        startingDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        if (selectAction.GetStateDown(trackedObjRight.inputSource))
        {
            dragging = false;
            Ray ray = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                dragging = true;
            }
        }     

        if (dragging && selectAction.GetState(trackedObjRight.inputSource))
        {
            Ray ray = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            RaycastHit hit;
            float curDistance = Vector3.Distance(Camera.main.transform.position, transform.position) - startingDistance;

            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                transform.parent = pointerRight.transform;

                if (moveValue.axis.y > 0) 
                {
                    transform.localScale *= 1.02f * curDistance;
                    //transform.position = (Camera.main.transform.position - pointerRight.transform.position) / 2f;
                }
                    
                else if (moveValue.axis.y < 0)
                    transform.localScale /= 1.02f;                
            }
        }
        else if (selectAction.GetStateUp(trackedObjRight.inputSource))
        {
            dragging = false;
            transform.parent = null;
        }

        /*if (selectAction.GetStateDown(trackedObjLeft.inputSource))
        {
            dragging = false;
            Ray ray = new Ray(trackedObjLeft.transform.position, trackedObjLeft.transform.forward);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                dragging = true;
            }
        }

        if (dragging && selectAction.GetState(trackedObjLeft.inputSource))
        {
            Ray ray = new Ray(trackedObjLeft.transform.position, trackedObjLeft.transform.forward);
            RaycastHit hit;

            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                transform.parent = pointerLeft.transform;              

                if (moveValue.axis.x > 0)
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), (40 * Time.deltaTime));
            }
        }
        else if (selectAction.GetStateUp(trackedObjLeft.inputSource))
        {
            dragging = false;
            transform.parent = null;
        }*/


    }
}
