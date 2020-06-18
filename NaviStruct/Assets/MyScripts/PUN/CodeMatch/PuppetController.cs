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
    //[HideInInspector]
    public GameObject avatarPlayer;
    [SerializeField]
    private GameObject avatarTorso;

    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private Camera thirdPersonCam;    

    private VRRig vrRig;    
    private VRPlayerMovement vrPlayerMove;
    private StandalonePlayerMoveController standalonePlayerMove;
    private PhotonView pv;
    private int spawnPicker;

    private void OnEnable()
    {
        if (MasterManager.ClassReference.PuppetController == null)
            MasterManager.ClassReference.PuppetController = this;
        CreatePlayer();
    }

    private void Start()
    {
        pv = avatarPlayer.GetComponent<PhotonView>();
        transform.position = avatarPlayer.transform.position;

        if (avatarPlayer.GetComponent<VRRig>() != null)
        {
            thirdPersonCam.gameObject.SetActive(false);
            if (pv.IsMine)
            {                
                vrPlayerMove = avatarPlayer.GetComponent<VRPlayerMovement>();
                vrRig = avatarPlayer.GetComponent<VRRig>();                

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
        else if(avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            head.GetComponent<Camera>().enabled = false;
            if (pv.IsMine)
            {
                standalonePlayerMove = avatarPlayer.GetComponent<StandalonePlayerMoveController>();
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
            vrPlayerMove.PositionController();
            //vrPlayerMove.StartHeadsetMoveAnimations();            
            vrPlayerMove.CheckForInput();
            vrPlayerMove.ApplyGravity();            
        }      
        else if(pv.IsMine && avatarPlayer.GetComponent<StandalonePlayerMoveController>() != null)
        {
            standalonePlayerMove.GetInput();
            standalonePlayerMove.CalculateDirection();
            standalonePlayerMove.Rotate();
            standalonePlayerMove.Move();
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
