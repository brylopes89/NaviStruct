using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR.InteractionSystem;

public class XRSelectHandler : MonoBehaviour
{
    public float smoothPosition = 6f;
    public float time = 2f;

    public bool is_Released = true;
    public bool has_Returned = true;

    private XRRayInteractor[] ray_Interactors = null;
    private XRBaseInteractable _interactable;
    private Rigidbody object_RB;
    private Vector3 original_Pos;
    private Quaternion original_Rot;

    void Awake()
    {
        ray_Interactors = GetComponentsInChildren<XRRayInteractor>();      
        //original_Pos = 
    }

    private void OnEnable()
    {
        foreach(XRRayInteractor ray_Interactor in ray_Interactors)
        {
            ray_Interactor.onSelectEnter.AddListener(OnSelectEnterHandler);
            ray_Interactor.onSelectExit.AddListener(OnSelectExitHandler);
            ray_Interactor.onHoverEnter.AddListener(OnHoverEnterHandler);
        }       
    }

    private void OnDisable()
    {
        foreach (XRRayInteractor ray_Interactor in ray_Interactors)
        {
            ray_Interactor.onSelectEnter.RemoveListener(OnSelectEnterHandler);
            ray_Interactor.onSelectExit.RemoveListener(OnSelectExitHandler);
            ray_Interactor.onHoverEnter.RemoveListener(OnHoverEnterHandler);
        }            
    }

    public void OnHoverEnterHandler(XRBaseInteractable interactable)
    {
        if (!interactable.CompareTag("Floor") && has_Returned)
        {
            _interactable = interactable;
            object_RB = _interactable.gameObject.GetComponent<Rigidbody>();
            original_Pos = _interactable.gameObject.transform.position;
            original_Rot = _interactable.gameObject.transform.rotation;
        }
    }

    public void OnSelectEnterHandler(XRBaseInteractable interactable)
    {        
        is_Released = false;
        has_Returned = false;
    }

    public void OnSelectExitHandler(XRBaseInteractable interactable)
    {
        is_Released = true;              
    }

    private void FixedUpdate()
    {
        if (is_Released && !has_Returned && object_RB != null)
        {
            StartCoroutine(InteractableMove());
        }            
    }

    private IEnumerator InteractableMove()
    {
        float i = 0.0f;
        float rate = (1.0f / time) * smoothPosition;

        while (i < 1)
        {
            i += Time.deltaTime * rate;

            _interactable.enabled = false;
            object_RB.MovePosition(Vector3.Lerp(_interactable.gameObject.transform.position, original_Pos, i));
            object_RB.MoveRotation(Quaternion.Slerp(_interactable.gameObject.transform.rotation, original_Rot, i));

            yield return null;
        }                   

        if(i == 1)
        {
            has_Returned = true;
            _interactable.enabled = true;
        }

        yield return null;
    }
}
