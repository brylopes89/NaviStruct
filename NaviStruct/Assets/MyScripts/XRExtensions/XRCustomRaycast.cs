using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Valve.VR.InteractionSystem;

public class XRCustomRaycast : MonoBehaviourPun
{
    private PlayerStateManager state_Manager = null;      
    private XRInteractorLineVisual line_Visual = null;
    private LineRenderer line_Renderer = null;

    private Vector3[] positions;

    private InteractableEventManager interactable = null;
    private List<InteractableEventManager> interactables = new List<InteractableEventManager>();    

    private int interact_Mask = 1 << 9;
    private PhotonView pv;

    private void Start()
    {
        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();
        state_Manager = MasterManager.ClassReference.Avatar.GetComponent<PlayerStateManager>();  
        line_Visual = this.GetComponent<XRInteractorLineVisual>();
        line_Renderer = this.GetComponent<LineRenderer>();
        positions = new Vector3[2]; 
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            RaycastForInteractable();
            SetImmersiveInteractables();
        }        
    }

    private void RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, this.transform.forward);        

        if (Physics.Raycast(ray, out hit, line_Visual.lineLength, interact_Mask))
        {          
            if (state_Manager.currentState == PlayerStates.Diorama)
            {
                if (!hit.collider.gameObject.GetComponent<InteractableEventManager>())
                {                    
                    hit.collider.gameObject.AddComponent<InteractableEventManager>();

                    interactable = hit.collider.gameObject.GetComponent<InteractableEventManager>();

                    interactable.GetComponent<Rigidbody>().isKinematic = true;
                    interactable.attachEaseInTime = 2.0f;
                    interactable.smoothPosition = true;
                    interactable.smoothRotation = true;

                    interactables.Add(interactable);
                }                                        
            }
        }        

        if (Physics.Raycast(ray, out hit, line_Visual.lineLength))
        {
            if ( !state_Manager.IsStateChange)
                DisplayLine(true, hit.point);
            else
                DisplayLine(false, transform.position);
        }      
    }

    private void SetImmersiveInteractables()
    {
        if (state_Manager.currentState == PlayerStates.Immersive)
        {          
            if (interactables.Count > 0)
            {
                for (int i = 0; i < interactables.Count; i++)
                {
                    Destroy(interactables[i]);
                    Destroy(interactables[i].GetComponent<Rigidbody>());                   
                }

                interactables.Clear();
            }
        }
    }

    private void DisplayLine(bool display, Vector3 endpoint)
    {
        line_Visual.enabled = display;
        line_Visual.lineLength = 8f;

        positions[0] = transform.position;
        positions[1] = endpoint;
        line_Renderer.SetPositions(positions);        
    }
}
