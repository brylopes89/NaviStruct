using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.XR;

public class PuppetController : MonoBehaviour
{
    public static PuppetController pc;

    [SerializeField]
    private GameObject head;
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
    private PhotonView pv;
    private int spawnPicker;

    private void Awake()
    {
        if (PuppetController.pc == null)
            PuppetController.pc = this;

        CreatePlayer();
        pv = avatarPlayer.GetComponent<PhotonView>();
        locomotion = GetComponent<VRPlayerLocomotion>();

        if (avatarPlayer.GetComponent<VRRig>() != null)
        {
            thirdPersonCam.gameObject.SetActive(false);
            vrRig = avatarPlayer.GetComponent<VRRig>();
            transform.position = avatarPlayer.transform.position;
            vrRig.head.vrTarget = head.transform;
            vrRig.leftHand.vrTarget = leftController.transform;
            vrRig.rightHand.vrTarget = rightController.transform;

            if (!pv.IsMine)
                head.GetComponent<Camera>().enabled = false;
        }
        else
        {
            thirdPersonCam.gameObject.SetActive(true);
            thirdPersonCam.gameObject.GetComponent<CameraController>().target = avatarPlayer.transform;
            locomotion.characterController = avatarPlayer.GetComponent<CharacterController>();
        }
    }

    private void FixedUpdate()
    {       
        locomotion.PositionController();
        locomotion.CheckForInput();
        locomotion.ApplyGravity();        
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
    }
}
