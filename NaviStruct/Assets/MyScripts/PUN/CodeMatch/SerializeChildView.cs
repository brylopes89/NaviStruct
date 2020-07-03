using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerializeChildView : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private Transform[] childTransforms;    

    void Start()
    {
        childTransforms = GetComponentsInChildren<Transform>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            foreach(Transform childTran in childTransforms)
            {
                stream.SendNext(childTran.localPosition);
                stream.SendNext(childTran.localRotation);
                stream.SendNext(childTran.localScale);
            }            
        }
        else
        {            
            foreach (Transform childTran in childTransforms)
            {                    
                childTran.localPosition = (Vector3)stream.ReceiveNext();
                childTran.localRotation = (Quaternion)stream.ReceiveNext();
                childTran.localScale = (Vector3)stream.ReceiveNext();
            }               
        }        
    }
}
