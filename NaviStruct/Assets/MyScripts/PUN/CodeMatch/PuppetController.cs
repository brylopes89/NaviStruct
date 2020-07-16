using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

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
    private PhotonView pv;

    private XRController[] xrControllers;
    private List<Transform> modelHand = new List<Transform>();

    private bool isHMDTracking;
    private bool isTeleport;

    private void Awake()
    {
        if (MasterManager.ClassReference.PuppetController == null)
            MasterManager.ClassReference.PuppetController = this;

        xrControllers = GetComponentsInChildren<XRController>();

        for (int i = 0; i < xrControllers.Length; i++)
        {
            modelHand.Add(xrControllers[i].modelPrefab);
        }
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

                for (int i = 0; i < xrControllers.Length; i++)
                {
                    xrControllers[i].modelPrefab = modelHand[i];
                }
            }                     
            else if(setupController.avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
            {               
                interactionManager.SetActive(false);
                head.GetComponent<Camera>().enabled = false;
                thirdPersonCam.gameObject.SetActive(true);
            }
        }

        else
        {
            head.GetComponent<Camera>().enabled = false;
            thirdPersonCam.gameObject.SetActive(false);

            if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
            {
                for (int i = 0; i < xrControllers.Length; i++)
                {
                    xrControllers[i].modelPrefab = null;
                }
            }                
        }        
    }

    private void Update()
    {
        if (!pv.IsMine)
            return;
        
        if (setupController.avatarPlayer.GetComponent<VRRig>() != null)
        {
            if (isHMDTracking)
            {
                //vrPlayerMove.CalulcateHMDVelocity();                  
            }
            else if (isTeleport)
            {
                //GetComponent<TeleportationProvider>().enabled = true;
            }
            else
            {
                //vrPlayerMove.CheckForInput();
                //GetComponent<TeleportationProvider>().enabled = false;
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
