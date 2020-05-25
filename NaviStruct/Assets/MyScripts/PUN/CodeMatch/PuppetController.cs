using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PuppetController : MonoBehaviour
{
    public GameObject head;
    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject rightController;
    [SerializeField]
    private Transform[] spawnPoints;
    [HideInInspector]
    public GameObject avatarPlayer;
    [SerializeField]
    private Camera thirdPersonCam;

    private VRRig vrRig;
    private VRPlayerLocomotion locomotion;
    private PlayerMoveController playerMove;
    private PhotonView pv;
    private int spawnPicker;

    private void OnEnable()
    {
        if (MasterManager.ClassReference.PuppetController == null)
            MasterManager.ClassReference.PuppetController = this;

        CreatePlayer();
        pv = avatarPlayer.GetComponent<PhotonView>();
        
        if (avatarPlayer.GetComponent<VRRig>() != null)
        {
            thirdPersonCam.gameObject.SetActive(false);
            if (pv.IsMine)
            {                
                locomotion = avatarPlayer.GetComponent<VRPlayerLocomotion>();
                vrRig = avatarPlayer.GetComponent<VRRig>();

                locomotion.controllers.Add(leftController.GetComponent<XRController>());

                vrRig.head.vrTarget = head.transform;
                vrRig.leftHand.vrTarget = leftController.transform;
                vrRig.rightHand.vrTarget = rightController.transform;
            }    
            else
            {
                head.GetComponent<Camera>().enabled = false;
            }                
        }
        else if(avatarPlayer.GetComponent<PlayerMoveController>() != null)
        {            
            if (pv.IsMine)
            {
                playerMove = avatarPlayer.GetComponent<PlayerMoveController>();
                thirdPersonCam.gameObject.SetActive(true);
            }                                      
            else
            {
                head.GetComponent<Camera>().enabled = false;
                thirdPersonCam.gameObject.SetActive(false);
            }                             
        }
    }

    private void Update()
    {
        if (pv.IsMine && avatarPlayer.GetComponent<VRRig>() != null)
        {
            locomotion.PositionController();
            locomotion.CheckForInput();
            locomotion.ApplyGravity();
        }      
        else if(pv.IsMine && avatarPlayer.GetComponent<PlayerMoveController>() != null)
        {
            playerMove.GetInput();
            playerMove.CalculateDirection();
            playerMove.Rotate();
            playerMove.Move();
        }
    }

    private void CreatePlayer()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        if (XRSettings.enabled)
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_VR"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;            
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;
        }

        MasterManager.ClassReference.Avatar = avatarPlayer;
    }
}
