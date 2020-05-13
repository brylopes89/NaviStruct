using PolyPerfect;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPlayerLocomotion : LocomotionProvider
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
    
    private AnimationController animationController;   

    // Start is called before the first frame update
    void Start()
    {
        animationController = AnimationController.instance;
        PositionController();
        animationController.SetAvatarAnimationIdle();       
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
    }

    private void CheckForMovement(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 pos))
            StartMoveWithVRDevices(pos);

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool isTouched))
        {
            speed = 3f;
            ApplyMovementAnimation(isTouched, "isWalking");
        }            

        else if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isPressed))
        {
            speed = 6f;
            ApplyMovementAnimation(isPressed, "isRunning");
        }
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

    public void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity * Time.deltaTime);
    }

    private void ApplyMovementAnimation(bool isMoving, string animName)
    {
        if (isMoving)
            animationController.SetAvatarAnimation(animName);
        else
            animationController.SetAvatarAnimationIdle();
    }
}

