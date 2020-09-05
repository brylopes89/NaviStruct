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
    public TeleportationProvider tp_Provider;

    private void Start()
    {
        state_Manager = MasterManager.ClassReference.Avatar.GetComponent<PlayerStateManager>();  

        line_Visual = this.GetComponent<XRInteractorLineVisual>();
        line_Renderer = this.GetComponent<LineRenderer>();       

        positions = new Vector3[2]; 
    }

    // Update is called once per frame
    void Update()
    {
        if (MasterManager.ClassReference.Avatar.GetComponent<PhotonView>().IsMine)
        {
            RaycastForInteractable();
            ResetInteractableComponents();
        }        
    }

    private void RaycastForInteractable()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, this.transform.forward);        

        if (Physics.Raycast(ray, out hit, line_Visual.lineLength, interact_Mask))
        {            
            DisplayLine(true, hit.point);

            if (state_Manager.currentState == PlayerStates.Diorama)
            {
                if (hit.collider.CompareTag("Teleportation"))
                {
                    tp_Provider.enabled = false;                    
                }
                else
                {
                    tp_Provider.enabled = true;
                }

                if (!hit.collider.GetComponent<InteractableEventManager>() && !hit.collider.CompareTag("Teleportation"))
                {                    
                    hit.collider.gameObject.AddComponent<InteractableEventManager>();
                    interactable = hit.collider.GetComponent<InteractableEventManager>();

                    interactable.GetComponent<Rigidbody>().isKinematic = true;
                    interactable.attachEaseInTime = 2.0f;
                    interactable.smoothPosition = true;
                    interactable.smoothRotation = true;

                    interactables.Add(interactable);
                }
            }
        }

        if (state_Manager.IsStateChange)
        {
            DisplayLine(false, this.transform.position);
        }
        else
        {
            DisplayLine(true, transform.forward);
        }
    }

    private void ResetInteractableComponents()
    {
        if (state_Manager.currentState == PlayerStates.Immersive)
        {
            tp_Provider.enabled = true;
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
