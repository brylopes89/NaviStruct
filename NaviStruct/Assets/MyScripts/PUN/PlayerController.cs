using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    [SerializeField]
    private Camera m_Camera;
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
             //playerGlobal = GameObject.FindGameObjectWithTag("MainCamera").transform;
             //playerLocal = playerGlobal.Find("Main Camera");

             //this.transform.SetParent(playerLocal);
             //this.transform.localPosition = Vector3.zero;

             avatar.SetActive(false);
         }

        if (!photonView.IsMine)
            m_Camera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) 
        { 
            
        }
        
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
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
        }*/
    }
}
