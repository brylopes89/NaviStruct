using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectSelector : MonoBehaviour
{
    public SteamVR_Behaviour_Pose trackedObjRight;
    public SteamVR_Behaviour_Pose trackedObjLeft;
    public SteamVR_Action_Boolean selectAction;       
    public SteamVR_Action_Vector2 moveValue;
    public GameObject pointerRight;
    public GameObject pointerLeft;
    
    public float maxSpeed = 1.0f;
    public float duration = 5f;

    // private float rotSpeed = 0.5f;
    private Vector3 minScale;
    private bool dragging;
    private Collider col;
   

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();        
        minScale = transform.localScale;
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

        if (selectAction.GetStateUp(trackedObjRight.inputSource))
        {
            dragging = false;
            transform.parent = null;
        }

        if (dragging && selectAction.GetState(trackedObjRight.inputSource))
        {
            Ray ray = new Ray(trackedObjRight.transform.position, trackedObjRight.transform.forward);
            RaycastHit hit;
            Vector3 changeScale = new Vector3(.001f, .001f, .001f);
            Vector3 maxScale = new Vector3(.01f, .01f, .01f);
            Vector3 minScale = new Vector3(.0009f, .0009f, .0009f);
            Vector3 currentScale = transform.localScale;                   

            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                             
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
                //transform.parent = pointerRight.transform;

                if (moveValue.GetAxis(trackedObjRight.inputSource).y > 0)
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
        }        

        if (selectAction.GetStateDown(trackedObjLeft.inputSource))
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

        if (selectAction.GetStateUp(trackedObjLeft.inputSource))
        {
            dragging = false;
            transform.parent = null;
        }

        if (dragging && selectAction.GetState(trackedObjLeft.inputSource))
        {
            Ray ray = new Ray(trackedObjLeft.transform.position, trackedObjLeft.transform.forward);
            RaycastHit hit;
            Vector3 rotSpeed = new Vector3(1, 1, 1);
            if (col.Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = col.ClosestPointOnBounds(point);
               // transform.parent = pointerLeft.transform;

                if (moveValue.GetAxis(trackedObjLeft.inputSource).x > 0)
                    transform.Rotate(rotSpeed);

                //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), (40 * Time.deltaTime));
            }
        }
        


    }
}
