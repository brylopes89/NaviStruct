using UnityEngine;
using Photon.Pun;
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
    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        avatarPlayer = GameSetupController.gameSetup.avatarPrefab;
        pv = avatarPlayer.GetComponent<PhotonView>();
        vrRig = avatarPlayer.GetComponent<VRRig>();

        vrRig.head.vrTarget = head.transform;
        vrRig.leftHand.vrTarget = leftController.transform;
        vrRig.rightHand.vrTarget = rightController.transform;
        avatarPlayer.transform.position = transform.position;

        if (!pv.IsMine)
            Destroy(head.GetComponent<Camera>());        
    }
}
