using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        if(vrTarget != null)
        {
            rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }       
    }
}

public class VRRig : MonoBehaviour
{   
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform headConstraint;
    public Vector3 headBodyOffset;    
    
    private PhotonView photonView;
    private PhotonView[] childrenPhotonView;

    // Start is called before the first frame update
    void Start()
    {        
        photonView = GetComponent<PhotonView>();
        childrenPhotonView = GetComponentsInChildren<PhotonView>();
        headBodyOffset = transform.position - headConstraint.position;        
    }
    
    void FixedUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;

        head.Map();
        leftHand.Map();
        rightHand.Map();

        //if (photonView.IsMine)
        //{
        //    transform.position = headConstraint.position + headBodyOffset;
        //    transform.forward = Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized;

        //    foreach (PhotonView child in childrenPhotonView)
        //    {
        //        if (child.IsMine)
        //        {
        //            head.Map();
        //            leftHand.Map();
        //            rightHand.Map();
        //        }
        //    }            
        //}        
    }
}
