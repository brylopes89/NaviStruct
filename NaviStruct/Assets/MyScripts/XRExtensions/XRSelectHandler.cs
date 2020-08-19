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

    public bool is_Released;
    public bool has_Returned;

    private XRRayInteractor[] ray_Interactors = null;
    private XRBaseInteractable _selectedInteractable;
    private XRBaseInteractable _hoveredInteractable;
    private int interact_Mask = 1 << 9;

    private Rigidbody object_RB;
    private Vector3 original_Pos;
    private Quaternion original_Rot;   

    void Awake()
    {
        ray_Interactors = GetComponentsInChildren<XRRayInteractor>();
        is_Released = true;
        has_Returned = true;
    }

    private void OnEnable()
    {
        //foreach(XRRayInteractor ray_Interactor in ray_Interactors)
        //{
        //    ray_Interactor.onSelectEnter.AddListener(OnSelectEnterHandler);
        //    ray_Interactor.onSelectExit.AddListener(OnSelectExitHandler);
        //    ray_Interactor.onHoverEnter.AddListener(OnHoverEnterHandler);
        //}       
    }

    private void OnDisable()
    {
        //foreach (XRRayInteractor ray_Interactor in ray_Interactors)
        //{
        //    ray_Interactor.onSelectEnter.RemoveListener(OnSelectEnterHandler);
        //    ray_Interactor.onSelectExit.RemoveListener(OnSelectExitHandler);
        //    ray_Interactor.onHoverEnter.RemoveListener(OnHoverEnterHandler);
        //}            
    }

    public void OnHoverEnterHandler(XRBaseInteractable interactable)
    {            
        if (interactable.gameObject.CompareTag("Floor"))
            return;         
        
        _hoveredInteractable = interactable;

        if (_hoveredInteractable == _selectedInteractable)
        {
            if (!has_Returned)
                _selectedInteractable.enabled = false;
            else
                _selectedInteractable.enabled = true;
        }        
    }

    public void OnSelectEnterHandler(XRBaseInteractable interactable)
    {
        is_Released = false;
        has_Returned = false;

        _selectedInteractable = interactable;

        object_RB = _selectedInteractable.gameObject.GetComponent<Rigidbody>();
        original_Pos = _selectedInteractable.gameObject.transform.position;
        original_Rot = _selectedInteractable.gameObject.transform.rotation;                    
    }

    public void OnSelectExitHandler(XRBaseInteractable interactable)
    {
        is_Released = true;

        if (_selectedInteractable.transform.position != original_Pos)
            has_Returned = false;
        else
            has_Returned = true;
    }

    private void FixedUpdate()
    {
        if (is_Released && !has_Returned)
        {
            //StartCoroutine(InteractableMove());
        }            
    }

    private IEnumerator InteractableMove()
    {
        float i = 0.0f;
        float rate = (1.0f / time) * smoothPosition;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            
            object_RB.MovePosition(Vector3.Lerp(_selectedInteractable.gameObject.transform.position, original_Pos, i));
            object_RB.MoveRotation(Quaternion.Slerp(_selectedInteractable.gameObject.transform.rotation, original_Rot, i));

            yield return null;
        }

        yield return new WaitForSeconds(2f);
        has_Returned = true;
    }
}
