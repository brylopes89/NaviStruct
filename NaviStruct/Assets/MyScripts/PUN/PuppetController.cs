using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public class PuppetController : MonoBehaviour
{
    [SerializeField]
    private GameObject head;
    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject rightController;

    private GameObject avatarPlayer;
    private VRRig vrRig;
    private PlayerLocomotion locomotion;
    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        avatarPlayer = GameSetupController.gameSetup.avatarPrefab;
        pv = avatarPlayer.GetComponent<PhotonView>();
        vrRig = avatarPlayer.GetComponent<VRRig>();
        //locomotion = avatarPlayer.GetComponent<AvatarLocomotion>();     
        locomotion = GetComponent<PlayerLocomotion>();

        if (!pv.IsMine)
            head.GetComponent<Camera>().enabled = false;   

        if (pv.IsMine)
        {
            vrRig.head.vrTarget = head.transform;
            vrRig.leftHand.vrTarget = leftController.transform;
            vrRig.rightHand.vrTarget = rightController.transform;
            transform.position = avatarPlayer.transform.position;
                                 
            //locomotion.characterController = avatarPlayer.GetComponent<CharacterController>();
        }
    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            locomotion.PositionController();
            locomotion.CheckForInput();
            locomotion.ApplyGravity();
        }
    }
}
