using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPlayerMovement : LocomotionProvider
{
    [Header("Locomotion Values")]
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float gravityMultiplier = 1.0f;
     
    [HideInInspector] 
    public List<XRController> controllers;   

    public float speedThreshold;
    [Range(0, 1)]
    public float smoothing;

    private float speedSmoothTime = 0.1f;
    private float walkSpeed = 3f;
    private float runSpeed = 8f;
    private float speedSmoothVelocity = 0f;
    private float currentVelocity;

    public GameObject puppetController;
    private VRRig vrRig;
    private Vector3 previousPos;
    private Vector3 movement;
    private Animator avatarAnim;

    private GameObject playerHead;
    private AnimationController animController;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        //animController = MasterManager.ClassReference.AnimController;
        avatarAnim = GetComponent<Animator>();
        //characterController = MasterManager.ClassReference.PuppetController.gameObject.GetComponent<CharacterController>();       
        //system = MasterManager.ClassReference.PuppetController.gameObject.GetComponent<LocomotionSystem>();
        //playerHead = MasterManager.ClassReference.PuppetController.head;

        //characterController = puppetController.GetComponent<CharacterController>();
        characterController = GetComponent<CharacterController>();
        vrRig = GetComponent<VRRig>();
        system = puppetController.GetComponent<LocomotionSystem>();
        playerHead = puppetController.GetComponent<PuppetController>().head;
        previousPos = vrRig.head.vrTarget.position;
    }

    public void StartHeadsetMoveAnimations()
    {
        //Compute the speed of headset
        Vector3 headsetSpeed = (vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;
        //Local speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = vrRig.head.vrTarget.position;

        //Set Animator Values
        float previousDirectionX = avatarAnim.GetFloat("DirectionX");
        float previousDirectionY = avatarAnim.GetFloat("DirectionY");
        
        avatarAnim.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        avatarAnim.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        avatarAnim.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }

    #region Trackpad Locomotion
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
                CheckForInputMovement(controller.inputDevice);            
        }        
    }

    private void CheckForInputMovement(InputDevice device)
    {
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 pos))
        {
            StartLocomotionWithVRDevices(pos, device);           
        }        
    }

    private void StartLocomotionWithVRDevices(Vector2 position, InputDevice device)
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
        //if (device.IsPressed(InputHelpers.Button.Primary2DAxisClick, out _))
        //    animController.SetVRAvatarFloatAnimation("MovementSpeed", 1f * position.magnitude, speedSmoothTime);
        //else
        //    animController.SetVRAvatarFloatAnimation("MovementSpeed", .5f * position.magnitude, speedSmoothTime);
    }

    public void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);        
        gravity.y *= Time.deltaTime;        

        characterController.Move(gravity * Time.deltaTime);
    }
    #endregion  
}

