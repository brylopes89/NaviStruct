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
        locomotion = GetComponent<PlayerLocomotion>();
        vrRig.head.vrTarget = head.transform;
        vrRig.leftHand.vrTarget = leftController.transform;
        vrRig.rightHand.vrTarget = rightController.transform;
        transform.position = avatarPlayer.transform.position;               
    }

    private void FixedUpdate()
    {       
        locomotion.PositionController();
        locomotion.CheckForInput();
        locomotion.ApplyGravity();        
    }    
}
