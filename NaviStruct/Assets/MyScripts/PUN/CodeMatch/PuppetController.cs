using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class PuppetController : MonoBehaviour
{    
    public GameObject head;
    public GameObject leftController;
    public GameObject rightController;
    public GameObject interactionManager;    

    public Camera thirdPersonCam;
    public GameSetupController setupController;    

    private VRRig vrRig;    
    private VRPlayerMovement vrPlayerMove;    
   
    private StandalonePlayerMoveController standalonePlayerMove;
    public PhotonView pv;

    private bool isLocomotion;
    private bool isHMDTracking;
    private bool isTeleport;

    private void Awake()
    {
        if (MasterManager.ClassReference.PuppetController == null)
            MasterManager.ClassReference.PuppetController = this;        
    }

    private void Start()
    {
        pv = setupController.avatarPlayer.GetComponent<PhotonView>();
       
        if (pv.IsMine)
        {
            if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
            {               
                interactionManager.SetActive(true);

                vrPlayerMove = setupController.avatarPlayer.GetComponent<VRPlayerMovement>();
                vrPlayerMove.controllers.Add(leftController.GetComponent<XRController>());
                vrRig = setupController.avatarPlayer.GetComponent<VRRig>();
                thirdPersonCam.gameObject.SetActive(false);

                transform.position = setupController.avatarPlayer.transform.position;
                vrRig.head.vrTarget = head.transform;
                vrRig.leftHand.vrTarget = leftController.transform;
                vrRig.rightHand.vrTarget = rightController.transform;         
                
            }                     
            else if(setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
            {
                standalonePlayerMove = setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>();
                interactionManager.SetActive(false);
                head.GetComponent<Camera>().enabled = false;
                thirdPersonCam.gameObject.SetActive(true);
            }
        }

        else
        {
            head.GetComponent<Camera>().enabled = false;
            thirdPersonCam.gameObject.SetActive(false);           
        }        
    }

    private void Update()
    {
        if (!pv.IsMine)
            return;
        
        if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
        {
            //vrPlayerMove.PositionCharacterController();
            //vrPlayerMove.CalulcateHMDVelocity();
            //vrPlayerMove.CheckForInput();
            //vrPlayerMove.ApplyGravity();

            if (isHMDTracking)
            {
                //vrPlayerMove.CalulcateHMDVelocity();                  
            }
            else if (isTeleport)
            {
                GetComponent<TeleportationProvider>().enabled = true;
            }
            else
            {
                //vrPlayerMove.CheckForInput();
                GetComponent<TeleportationProvider>().enabled = false;
            }
                
        }
        else if (setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            //standalonePlayerMove.GetInput();
            //standalonePlayerMove.CalculateDirection();
            //standalonePlayerMove.Rotate();
            //standalonePlayerMove.Move();
        }               
    }

    public void LocomotionToggleOnClick(bool isToggle)
    {
        isLocomotion = isToggle;        
    }

    public void TeleportToggleOnClick(bool isToggle)
    {
        isTeleport = isToggle;
    }

    public void HMDTrackingToggleOnClick(bool isToggle)
    {
        isHMDTracking = isToggle;
    }
}
