using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PuppetController : MonoBehaviour
{
    public static PuppetController pc;

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
    private PhotonView pv;
    private int spawnPicker;

    private void Awake()
    {
        if (pc == null)
            pc = this;

        CreatePlayer();
        pv = avatarPlayer.GetComponent<PhotonView>();        

        if (avatarPlayer.GetComponent<VRRig>() != null)
        {            
            thirdPersonCam.gameObject.SetActive(false);
            locomotion = avatarPlayer.GetComponent<VRPlayerLocomotion>();
            vrRig = avatarPlayer.GetComponent<VRRig>();

            locomotion.characterController = GetComponent<CharacterController>();
            locomotion.controllers.Add(leftController.GetComponent<XRController>());

            vrRig.head.vrTarget = head.transform;
            vrRig.leftHand.vrTarget = leftController.transform;
            vrRig.rightHand.vrTarget = rightController.transform;

            if (!pv.IsMine)
                head.GetComponent<Camera>().enabled = false;
        }
        else
        {
            thirdPersonCam.gameObject.SetActive(true);                      
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

        SceneManagerSingleton.instance.avatar = avatarPlayer;
    }
}
