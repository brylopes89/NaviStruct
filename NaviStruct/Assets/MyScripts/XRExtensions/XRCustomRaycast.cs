using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCustomRaycast : MonoBehaviour
{
    public float smoothPosition = 2f;    

    private TeleportationArea t_Area = null;
    private PlayerStateManager state_Manager = null;
    private XRGrabInteractable interactable = null;   
    private XRInteractorLineVisual line_Visual = null;
    private LineRenderer line_Renderer = null;

    private Vector3[] positions;
    private Vector3 original_Pos;
    private Quaternion original_Rot;
    private Rigidbody object_RB;

    private int interact_Mask = 1 << 9;
    private void Start()
    {
        t_Area = FindObjectOfType<TeleportationArea>();
        state_Manager = MasterManager.ClassReference.Avatar.GetComponent<PlayerStateManager>();        

        line_Visual = GetComponent<XRInteractorLineVisual>();
        line_Renderer = GetComponent<LineRenderer>();
        positions = new Vector3[2]; 
    }

    // Update is called once per frame
    void Update()
    {
        RaycastForInteractable();                 
    }

    private void RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);        

        if (Physics.Raycast(ray, out hit, line_Visual.lineLength, interact_Mask))
        {
            if (state_Manager.currentState == PlayerStates.Diorama)
            {
                if (hit.collider.gameObject.GetComponent<XRGrabInteractable>())
                {
                    t_Area.customReticle = null;
                    interactable = hit.collider.gameObject.GetComponent<XRGrabInteractable>();
                    object_RB = interactable.GetComponent<Rigidbody>();
                    original_Pos = interactable.transform.position;
                    original_Rot = interactable.transform.rotation;

                    if (interactable.transform.position != original_Pos)
                    {
                        //TODO: LERP INTERACTABLE BACK TO ORIGINAL POSITION
                        //object_RB.isKinematic = false;

                        //object_RB.transform.position = Vector3.Lerp(interactable.transform.position, original_Pos, smoothPosition * Time.deltaTime);
                        //interactable.transform.rotation = Quaternion.Slerp(interactable.transform.rotation, original_Rot, smoothPosition * Time.deltaTime);
                    }
                    else
                    {
                        //object_RB.isKinematic = true;
                    }
                }
            }                           
        }

        if (Physics.Raycast(ray, out hit, line_Visual.lineLength) && !state_Manager.IsStateChange)
        {            
            DisplayLine(true, hit.point);
        }
        else
        {
            DisplayLine(false, transform.position);
        }
    }

    private void DisplayLine(bool display, Vector3 endpoint)
    {
        line_Visual.enabled = display;
        line_Visual.lineLength = 6f;

        positions[0] = transform.position;
        positions[1] = endpoint;
        line_Renderer.SetPositions(positions);        
    }
}
