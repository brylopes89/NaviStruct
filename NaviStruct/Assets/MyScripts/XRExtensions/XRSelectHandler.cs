using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSelectHandler : MonoBehaviour
{
    public float smoothPosition = 5f;
    public bool isReleased;
    private XRRayInteractor ray_Interactor = null;

    private GameObject _interactable;
    private Rigidbody object_RB;
    private Vector3 original_Pos;
    private Quaternion original_Rot;

    void Awake()
    {
        ray_Interactor = GetComponent<XRRayInteractor>();
    }

    private void OnEnable()
    {
        ray_Interactor.onSelectEnter.AddListener(OnSelectEnterHandler);
        ray_Interactor.onSelectExit.AddListener(OnSelectExitHandler);
    }

    private void OnDisable()
    {
        ray_Interactor.onSelectEnter.RemoveListener(OnSelectEnterHandler);
        ray_Interactor.onSelectExit.RemoveListener(OnSelectExitHandler);        
    }

    public void OnSelectEnterHandler(XRBaseInteractable interactable)
    {
        isReleased = false;
        _interactable = interactable.gameObject;
        object_RB = _interactable.GetComponent<Rigidbody>();
        original_Pos = _interactable.transform.position;
        original_Rot = _interactable.transform.rotation;

        _interactable.GetComponent<XRGrabInteractable>().attachTransform = this.transform;        
    }

    public void OnSelectExitHandler(XRBaseInteractable interactable)
    {
        isReleased = true;              
    }

    private void FixedUpdate()
    {
        if (isReleased && _interactable.transform.position != original_Pos)
        {
            object_RB.MovePosition(Vector3.Lerp(_interactable.transform.position, original_Pos, Time.deltaTime * smoothPosition));
            object_RB.MoveRotation(Quaternion.Slerp(_interactable.transform.rotation, original_Rot, Time.deltaTime * smoothPosition));
        }
            
    }
}
