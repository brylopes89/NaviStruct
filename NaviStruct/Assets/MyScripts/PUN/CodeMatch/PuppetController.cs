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

        if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
        {
            vrPlayerMove = setupController.avatarPlayer.GetComponent<VRPlayerMovement>();
            vrPlayerMove.controllers.Add(leftController.GetComponent<XRController>());
            vrRig = setupController.avatarPlayer.GetComponent<VRRig>();
            thirdPersonCam.gameObject.SetActive(false);

            transform.position = setupController.avatarPlayer.transform.position;
            vrRig.head.vrTarget = head.transform;
            vrRig.leftHand.vrTarget = leftController.transform;
            vrRig.rightHand.vrTarget = rightController.transform;            

            if (!pv.IsMine)            
                head.GetComponent<Camera>().enabled = false;               
                                     
        }
        else if(setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            standalonePlayerMove = setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>();
            head.GetComponent<Camera>().enabled = false;
            interactionManager.SetActive(false);
            thirdPersonCam.gameObject.SetActive(true);

            if (!pv.IsMine)            
                thirdPersonCam.gameObject.SetActive(false);                                           
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
            {
                vrPlayerMove.PositionCharacterController();
                vrPlayerMove.CalulcateHMDVelocity();
                vrPlayerMove.CheckForInput();
                vrPlayerMove.ApplyGravity();

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
                standalonePlayerMove.GetInput();
                standalonePlayerMove.CalculateDirection();
                standalonePlayerMove.Rotate();
                standalonePlayerMove.Move();
            }
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
