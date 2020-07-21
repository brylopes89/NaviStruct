using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerializeChildView : MonoBehaviourPun, IPunObservable
{  
    public Transform[] childTransforms;

    private void Start()
    {
        PhotonNetwork.SendRate = 40; //20
        PhotonNetwork.SerializationRate = 40; //10   
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        foreach (Transform childTran in childTransforms)
        {
            if (stream.IsWriting)
            {               
                stream.SendNext(childTran.localPosition);
                stream.SendNext(childTran.localRotation);
                stream.SendNext(childTran.localScale);                
            }
            else
            {                
                childTran.localPosition = (Vector3)stream.ReceiveNext();
                childTran.localRotation = (Quaternion)stream.ReceiveNext();
                childTran.localScale = (Vector3)stream.ReceiveNext();                
            }
        }        
    }
}
