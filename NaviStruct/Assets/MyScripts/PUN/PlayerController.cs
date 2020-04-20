using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunObservable
{
    PhotonView photonView;

    public GameObject avatar;

    public Transform playerGlobal;
    public Transform playerLocal;

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;

    Vector3 realAvatarPosition = Vector3.zero;
    Quaternion realAvatarRotation = Quaternion.identity;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        photonView.ObservedComponents.Add(this);
        if (!photonView.IsMine)
            enabled = false;
    }

    void Start()
    {
         if (photonView.IsMine)
         {

             playerGlobal = GameObject.FindGameObjectWithTag("MainCamera").transform;
             playerLocal = playerGlobal.Find("Main Camera");

             this.transform.SetParent(playerLocal);
             this.transform.localPosition = Vector3.zero;

             avatar.SetActive(false);
         }        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) 
        { 
            transform.position = realPosition;
            Debug.Log("Transform Position: " + transform.position);
            Debug.Log("Real Position: " + realPosition);
            transform.rotation = realRotation;

            avatar.transform.position = realAvatarPosition;
            Debug.Log("Avatar Position: " + avatar.transform.position);
            Debug.Log("Real Avatar Position" + realAvatarPosition);
            avatar.transform.rotation = realAvatarRotation;

            realPosition = transform.position;
            realAvatarPosition = avatar.transform.position;
        }
        
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //we own this player: send the others our data
            stream.SendNext(transform.position); //position of player
            stream.SendNext(transform.rotation); //Rotation of player
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);
        }

        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realAvatarPosition = (Vector3)stream.ReceiveNext();
            realAvatarRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
