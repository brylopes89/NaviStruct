using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunObservable
{
    PhotonView photonView;
   
    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        photonView.ObservedComponents.Add(this);
        if (!photonView.IsMine)
            enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //we own this player: send the others our data
            stream.SendNext(transform.position); //position of player
            stream.SendNext(transform.rotation); //Rotation of player
        }
    }
}
