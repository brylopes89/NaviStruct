using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerController : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private GameObject avatar;   
   

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        if (photonView.IsMine)
        {
            
            avatar.SetActive(false);
        }

        if (!photonView.IsMine)
        {
           // m_Camera.enabled = false;            
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
