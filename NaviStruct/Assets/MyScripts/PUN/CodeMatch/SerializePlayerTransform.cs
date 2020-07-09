using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerializePlayerTransform : MonoBehaviour, IPunObservable
{
    private void Start()
    {
        PhotonNetwork.SendRate = 40; //20
        PhotonNetwork.SerializationRate = 40; //10       
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {            
            stream.SendNext(transform.localPosition);            
            stream.SendNext(transform.localRotation);                       
        }
        else
        {            
            transform.localPosition = (Vector3)stream.ReceiveNext();            
            transform.localRotation = (Quaternion)stream.ReceiveNext();                         
        }
    }
}
