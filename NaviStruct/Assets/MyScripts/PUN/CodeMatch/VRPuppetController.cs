using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class VRPuppetController : MonoBehaviour
{    
    [SerializeField]
    private GameObject head;
    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject rightController;
    [SerializeField]
    private GameSetupController setupController;    

    private VRRig vrRig;    
    private VRPlayerMovement vrPlayerMove;    
    private PhotonView pv;

    private void Awake()
    {
        if (MasterManager.ClassReference.VRPuppetController == null)
            MasterManager.ClassReference.VRPuppetController = this;         
    }

    private void Start()
    {
        pv = setupController.avatarPlayer.GetComponent<PhotonView>();        
       
        if (pv.IsMine)
        {
            if (MasterManager.ClassReference.IsVRSupport)
            {   
                vrPlayerMove = setupController.avatarPlayer.GetComponent<VRPlayerMovement>();
                vrPlayerMove.controllers.Add(leftController.GetComponent<XRController>());
                vrRig = setupController.avatarPlayer.GetComponent<VRRig>();                

                transform.position = setupController.avatarPlayer.transform.position;
                vrRig.head.vrTarget = head.transform;
                vrRig.leftHand.vrTarget = leftController.transform;
                vrRig.rightHand.vrTarget = rightController.transform;               
            }            
        }
        else
        {
            head.GetComponent<Camera>().enabled = false;                                
        }        
    }
}
