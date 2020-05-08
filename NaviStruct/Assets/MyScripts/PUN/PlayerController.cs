using PolyPerfect;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class PlayerController : LocomotionProvider
{
    [Header("Locomotion Values")]
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float gravityMultiplier = 1.0f;

    [Header("Device For Movement")]  
    [SerializeField]
    private List<XRController> controllers;    

    [SerializeField]
    private CharacterController characterController;
    private GameObject head;    

    [SerializeField]
    private PhotonView myPhotonView;
    
    [SerializeField]
    private Camera cam;

    protected override void Awake()
    {                
        head = GetComponent<XRRig>().cameraGameObject;
        cam = gameObject.GetComponentInChildren<Camera>();        
    }

    // Start is called before the first frame update
    void Start()
    {
        PositionController();        
        AvatarAnimationController.animControl.SetAnimationIdle();

        if (!myPhotonView.IsMine && cam != null)
            cam.enabled = false;
        else if(myPhotonView.IsMine && cam != null)        
            cam.enabled = true;           
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (myPhotonView.IsMine)
        {
            PositionController();
            CheckForInput();
            ApplyGravity();
        }         
    }

    private void PositionController()
    {
        //Get the head height in local, playspace ground
        float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHeight;

        //Cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        //Let's move the player in local space as well
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;

        //Apply
        characterController.center = newCenter;
    }

    private void CheckForInput()
    {
        foreach(XRController controller in controllers)
        {
            if (controller.enableInputActions)            
                CheckForMovement(controller.inputDevice);           
        }
    }

    private void CheckForMovement(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 pos))  
            StartMove(pos);

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool isPressed))
            ApplyMovementAnimation(isPressed);

    }

    private void StartMove(Vector2 position)
    {
        //Apply the touch position to the head's forward Vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        //Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        //Apply speed and move
        Vector3 movement = direction * speed;        
        
        characterController.Move(movement * Time.deltaTime);                           
    }

    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity * Time.deltaTime);
    }

    private void ApplyMovementAnimation(bool isMoving)
    {
        if(isMoving)
            AvatarAnimationController.animControl.SetAnimation("isWalking");
        else
            AvatarAnimationController.animControl.SetAnimationIdle();
    }
}
