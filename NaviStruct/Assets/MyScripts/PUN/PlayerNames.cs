using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerNames : MonoBehaviourPun
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    public Vector3 offset;

    private PlayerAvatarManager avatarManager;
    private Vector3 targetPos;
    private GameObject playerRig;

    // Start is called before the first frame update
    void Start()
    {         
        nameText.text = this.photonView.Owner.NickName;
        avatarManager = this.GetComponent<PlayerAvatarManager>();
        playerRig = GameObject.Find("VR_Rig");
    }

    void Update()
    {         
        //Vector3 projected = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        //Vector3 targetPos = projected + Camera.main.transform.position;

        nameText.transform.LookAt(Camera.main.transform.localPosition);
        nameText.transform.Rotate(0, 180, 0);

        //Vector3 rotationOffset = Quaternion.LookRotation(nameText.gameObject.transform.position - avatarManager.rig.player_Rig.transform.position).eulerAngles;
        //nameText.gameObject.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
