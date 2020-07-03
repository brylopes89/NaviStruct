using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerializePlayerTransform : MonoBehaviour, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {           
            stream.SendNext(transform.localPosition);            
            stream.SendNext(transform.localRotation);
            stream.SendNext(transform.localScale);
            stream.SendNext(transform.parent.localScale);
        }
        else
        {            
            transform.localPosition = (Vector3)stream.ReceiveNext();
            transform.localRotation = (Quaternion)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();        
            transform.parent.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}
