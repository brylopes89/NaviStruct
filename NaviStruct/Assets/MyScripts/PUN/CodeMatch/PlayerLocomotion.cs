using PolyPerfect;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerLocomotion : LocomotionProvider
{
    [Header("Locomotion Values")]
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float gravityMultiplier = 1.0f;

    [Header("Player Devices")]
    [SerializeField]
    private GameObject playerHead;
    [SerializeField]
    private List<XRController> controllers;
    
    public CharacterController characterController;    

    // Start is called before the first frame update
    void Start()
    {
        PositionController();
        AvatarAnimationController.animControl.SetAnimationIdle();
    }

    public void PositionController()
    {
        //Get the head height in local, playspace ground
        float headHeight = Mathf.Clamp(playerHead.transform.localPosition.y, 1, 2);
        characterController.height = headHeight;

        //Cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        //Let's move the player in local space as well
        newCenter.x = playerHead.transform.localPosition.x;
        newCenter.z = playerHead.transform.localPosition.z;

        //Apply
        characterController.center = newCenter;
    }

    public void CheckForInput()
    {
        foreach (XRController controller in controllers)
        {
            if (controller.enableInputActions)
                CheckForMovement(controller.inputDevice);            
        }

        if(!XRDevice.isPresent || !XRSettings.isDeviceActive)
            StartMoveWithKeyPress();
    }

    private void CheckForMovement(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 pos))
            StartMoveWithVRDevices(pos);

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool isPressed))
            ApplyMovementAnimation(isPressed);

    }

    private void StartMoveWithVRDevices(Vector2 position)
    {
        //Apply the touch position to the head's forward Vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, playerHead.transform.eulerAngles.y, 0);

        //Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        //Apply speed and move
        Vector3 movement = direction * speed;

        characterController.Move(movement * Time.deltaTime);
    }

    private void StartMoveWithKeyPress()
    {        
        float jumpSpeed = 8.0f;        

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        direction *= speed;

        if (Input.GetButton("Jump"))
        {
            direction.y = jumpSpeed;
        }          
        characterController.Move(direction * Time.deltaTime);
    }
    public void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity * Time.deltaTime);
    }

    private void ApplyMovementAnimation(bool isMoving)
    {
        if (isMoving)
            AvatarAnimationController.animControl.SetAnimation("isWalking");
        else
            AvatarAnimationController.animControl.SetAnimationIdle();
    }
}

