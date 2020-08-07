using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSelectHandler : MonoBehaviour
{
    const float k_DefaultSmoothingAmount = 5f;
    [SerializeField, Range(0.0f, 20.0f)]
    float m_SmoothPositionAmount = k_DefaultSmoothingAmount;
   
    public float smoothPositionAmount { get { return m_SmoothPositionAmount; } set { m_SmoothPositionAmount = value; } }

    [SerializeField, Range(0.0f, 20.0f)]
    float m_SmoothRotationAmount = k_DefaultSmoothingAmount;    
    public float smoothRotationAmount { get { return m_SmoothRotationAmount; } set { m_SmoothRotationAmount = value; } }
    
    private XRRayInteractor ray_Interactor = null;
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
        original_Pos = interactable.transform.position;
        original_Rot = interactable.transform.rotation;
    }

    public void OnSelectExitHandler(XRBaseInteractable interactable)
    {
        if (interactable.transform.CompareTag("Playground"))
        {
            interactable.transform.position = original_Pos;
            interactable.transform.rotation = original_Rot;
        }             
    }
}
