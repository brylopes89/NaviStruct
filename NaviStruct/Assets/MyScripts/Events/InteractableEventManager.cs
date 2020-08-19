using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableEventManager : XRGrabInteractable
{      
    public float time = 2f;

    public bool is_Released;
    public bool has_Returned;

    XRGrabInteractable grab_Interactable;
    XRBaseInteractor base_Interactor;

    private Vector3 original_Pos;
    private Quaternion original_Rot;
    private Rigidbody rb;

    protected override void Awake()
    {       
        if(this.GetComponent<XRGrabInteractable>() != null)
            grab_Interactable = this.GetComponent<XRGrabInteractable>();               

        if (this.GetComponent<Rigidbody>() != null)
            rb = this.GetComponent<Rigidbody>();

        base.Awake();
    }

    private void Start()
    {        
        original_Pos = this.transform.position;
        original_Rot = this.transform.rotation;

        is_Released = true;
        has_Returned = true;
    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {        
        if (is_Released && has_Returned)
        {
            is_Released = false;
            has_Returned = false;
        }

        base_Interactor = interactor;
        grab_Interactable.attachTransform = base_Interactor.transform;

        base.OnSelectEnter(interactor);
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {        
        if (!is_Released)
            is_Released = true;

        grab_Interactable.attachTransform = null;

        base.OnSelectExit(interactor);
    }   

    private void FixedUpdate()
    {
        if (is_Released && !has_Returned)
        {
            StartCoroutine(InteractableMove());
        }
    }

    private IEnumerator InteractableMove()
    {
        float i = 0.0f;
        float rate = (1.0f / time) * this.smoothPositionAmount;

        while (i < 1)
        {
            i += Time.deltaTime * rate;

            rb.MovePosition(Vector3.Lerp(this.transform.position, original_Pos, i));
            rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, original_Rot, i));

            yield return null;
        }

        yield return new WaitForSeconds(time);
        has_Returned = true;
    }
}
