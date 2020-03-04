using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ObjectSelector : MonoBehaviour
{    
    public SteamVR_Behaviour_Pose trackedObjRight;
    public SteamVR_Behaviour_Pose trackedObjLeft;
    public SteamVR_Action_Boolean selectAction;       
    public SteamVR_Action_Vector2 moveValue;
    public GameObject pointerRight;
    public GameObject pointerLeft;
    public Hand hand;
    
    public float maxSpeed = 1.0f;
    public float duration = 5f;

    // private float rotSpeed = 0.5f;    
    private bool dragging;
    private Collider col;   

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        if (selectAction.GetStateDown(trackedObjRight.inputSource) || selectAction.GetStateDown(trackedObjLeft.inputSource))
        {
            dragging = false;
            //Ray ray = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            Ray rayR = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            Ray rayL = new Ray(trackedObjLeft.transform.position, trackedObjLeft.transform.forward);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (col.Raycast(rayR, out hit, Mathf.Infinity) || col.Raycast(rayL, out hit, Mathf.Infinity))
            {
                dragging = true;
               
            }
        }

        if (selectAction.GetStateUp(trackedObjRight.inputSource) || selectAction.GetStateUp(trackedObjLeft.inputSource))
        {
            dragging = false;
            transform.parent = null;
            
        }

        if (dragging && selectAction.GetState(trackedObjRight.inputSource) || selectAction.GetState(trackedObjLeft.inputSource))
        {
            Ray rayR = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            Ray rayL = new Ray(trackedObjLeft.transform.position, trackedObjLeft.transform.forward);
            RaycastHit hit;
            
            Vector3 maxScale = new Vector3(.01f, .01f, .01f);
            Vector3 minScale = new Vector3(.0009f, .0009f, .0009f);
            Vector3 newRotY = new Vector3(0, 1, 0);
            Vector3 newRotX = new Vector3(1, 0, 0);

            if (col.Raycast(rayR, out hit, Mathf.Infinity) || col.Raycast(rayL, out hit, Mathf.Infinity))
            {                             
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                // transform.position = hit.transform.position;                

                /*if (moveValue.GetAxis(trackedObjRight.inputSource).y > 0)
                {                    
                    transform.localScale *= 1.02f;

                    if (transform.localScale.x > .01f && transform.localScale.y > .01f && transform.localScale.z > 0.01f)
                        transform.localScale = maxScale;                                                     
                }
                    
                else if (moveValue.GetAxis(trackedObjRight.inputSource).y < 0)
                {
                    transform.localScale /= 1.02f;

                    if (transform.localScale.x < .0009f && transform.localScale.y < .0009f && transform.localScale.z < .0009f)
                        transform.localScale = minScale;
                }                               
            }

            if (col.Raycast(rayL, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                
                if (moveValue.GetAxis(trackedObjLeft.inputSource).x > 0)
                    transform.Rotate(newRotY);

                else if (moveValue.GetAxis(trackedObjLeft.inputSource).x < 0)
                    transform.Rotate(-newRotY);                
            }*/
            }
        }          
    }
}
