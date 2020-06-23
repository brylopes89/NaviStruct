using UnityEngine;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using System.IO;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PuppetController : MonoBehaviour
{    
    public GameObject head;
    public GameObject leftController;
    public GameObject rightController;
    public GameObject interactionManager;   

    public DropdownMultiSelect multiSelectMenu;
    public Camera thirdPersonCam;
    public GameSetupController setupController;    

    private VRRig vrRig;    
    private VRPlayerMovement vrPlayerMove;
    private StandalonePlayerMoveController standalonePlayerMove;
    private PhotonView pv;    

    private bool isLocomotion;
    private bool isHMDTracking;
    private bool isTeleport;

    private void OnEnable()
    {
        if (MasterManager.ClassReference.PuppetController == null)
            MasterManager.ClassReference.PuppetController = this;       
    }

    private void Start()
    {
        pv = setupController.avatarPlayer.GetComponent<PhotonView>();
        transform.position = setupController.avatarPlayer.transform.position;        

        if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
        {
            thirdPersonCam.gameObject.SetActive(false);
            if (pv.IsMine)
            {                
                vrPlayerMove = setupController.avatarPlayer.GetComponent<VRPlayerMovement>();
                vrRig = setupController.avatarPlayer.GetComponent<VRRig>();                

                vrPlayerMove.controllers.Add(leftController.GetComponent<XRController>());

                vrRig.head.vrTarget = head.transform;
                vrRig.leftHand.vrTarget = leftController.transform;
                vrRig.rightHand.vrTarget = rightController.transform;
            }    
            else
            {                
                head.GetComponent<Camera>().enabled = false;
            }                
        }
        else if(setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            standalonePlayerMove = setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>();
            head.GetComponent<Camera>().enabled = false;
            interactionManager.SetActive(false);

            if (pv.IsMine)
            {
                thirdPersonCam.gameObject.SetActive(true);                                          
            }                                      
            else
            {
                head.GetComponent<Camera>().enabled = false;
                thirdPersonCam.gameObject.SetActive(false);
            }                             
        }
    }

    private void FixedUpdate()
    {
        if (pv.IsMine && setupController.avatarPlayer.GetComponent<VRRig>() != null)
        {
            vrPlayerMove.PositionController();

            if (isHMDTracking)
            {
                //vrPlayerMove.StartHeadsetMoveAnimations();                  
            }                
            else if (isTeleport)
            {
                GetComponent<TeleportationProvider>().enabled = true;                  
            }
            else
            {
                vrPlayerMove.CheckForInput();
                GetComponent<TeleportationProvider>().enabled = false;
            }

            vrPlayerMove.ApplyGravity();            
        }      
        else if(pv.IsMine && setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            standalonePlayerMove.GetInput();
            standalonePlayerMove.CalculateDirection();
            standalonePlayerMove.Rotate();
            standalonePlayerMove.Move();
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
