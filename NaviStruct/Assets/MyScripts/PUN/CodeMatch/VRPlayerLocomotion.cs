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
     
    [HideInInspector] 
    public List<XRController> controllers;
    
    private CharacterController characterController;

    private float speedSmoothTime = 0.1f;
    private float walkSpeed = 3f;
    private float runSpeed = 8f;
    private float speedSmoothVelocity = 0f;
    private float currentVelocity;

    private Vector3 movement;
    private GameObject playerHead;
    private AnimationController animController;

    // Start is called before the first frame update
    void Start()
    {
        animController = SceneManagerSingleton.instance.animationController;
        characterController = SceneManagerSingleton.instance.puppetController.gameObject.GetComponent<CharacterController>();
        system = SceneManagerSingleton.instance.puppetController.gameObject.GetComponent<LocomotionSystem>();
        playerHead = SceneManagerSingleton.instance.puppetController.head;                
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
        {
            StartMoveWithVRDevices(pos, device);           
        }        
    }

    private void StartMoveWithVRDevices(Vector2 position, InputDevice device)
    {
        //Apply the touch position to the head's forward Vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, playerHead.transform.eulerAngles.y, 0);

        //Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        float targetSpeed = walkSpeed * position.magnitude;
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out _))
            targetSpeed = runSpeed * position.magnitude;        

        //Apply speed and move
        currentVelocity = Mathf.SmoothDamp(currentVelocity, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        movement = direction * currentVelocity;        
        characterController.Move(movement * Time.deltaTime);
        if (device.IsPressed(InputHelpers.Button.Primary2DAxisClick, out _))
            animController.SetAvatarFloatAnimation("MovementSpeed", 1f * position.magnitude, speedSmoothTime);
        else
            animController.SetAvatarFloatAnimation("MovementSpeed", .5f * position.magnitude, speedSmoothTime);
    }

    public void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);        
        gravity.y *= Time.deltaTime;        

        characterController.Move(gravity * Time.deltaTime);
    }
}

