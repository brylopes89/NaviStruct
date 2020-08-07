using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCustomRaycast : MonoBehaviour
{
    private TeleportationArea t_Area = null;
    private PlayerStateManager state_Manager = null;
    private XRGrabInteractable interactable = null;   
    private XRInteractorLineVisual line_Visual = null;
    private LineRenderer line_Renderer = null;

    private Vector3[] positions;   

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
                t_Area.customReticle = null;                

                if (hit.collider.gameObject.GetComponent<XRGrabInteractable>())
                {
                    interactable = hit.collider.gameObject.GetComponent<XRGrabInteractable>();
                    //interactable.attachTransform = this.transform;                  
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
