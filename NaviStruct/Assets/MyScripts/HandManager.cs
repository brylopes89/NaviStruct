using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandManager : MonoBehaviour
{
    public SteamVR_Action_Boolean m_TriggerAction = null;
    public SteamVR_Action_Boolean m_TouchPadAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private ObjectInteract m_CurrentInteractable = null;
    public List<ObjectInteract> m_ContactInteractables = new List<ObjectInteract>();

    [HideInInspector] public bool isTriggerPressed = false;
    private PhysicsPointer pointer;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
        pointer = GetComponentInChildren<PhysicsPointer>();
    }
    
    private void Update()
    {       
        if (m_TriggerAction.GetStateDown(m_Pose.inputSource))
        {            
            if(m_CurrentInteractable != null)
            {
                //m_CurrentInteractable.Action();
                return;
            }

            isTriggerPressed = true;
            Pickup(transform);
        }
        
        if (m_TriggerAction.GetStateUp(m_Pose.inputSource))
        {
            isTriggerPressed = false;
            Drop();            
        }                                          
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interact"))
            return;

        m_ContactInteractables.Add(other.gameObject.GetComponent<ObjectInteract>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interact"))
            return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<ObjectInteract>());
    }

    public void Pickup(Transform attachPos)
    {
        //Get nearest interactable
        m_CurrentInteractable = GetNearestInteractable();

        //Null check
        if (!m_CurrentInteractable)
            return;

        //Already held, check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        //Position 
        m_CurrentInteractable.ApplyOffset(attachPos);

        //Attach to fixed joint on hands
        
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;              

        //Set active hand
        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop()
    {
        //Null check
        if (!m_CurrentInteractable)
            return;

        //Apply veclocity
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

        //Detach from joint
        m_Joint.connectedBody = null;

        //Clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private ObjectInteract GetNearestInteractable()
    {
        ObjectInteract nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(ObjectInteract interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            
            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
