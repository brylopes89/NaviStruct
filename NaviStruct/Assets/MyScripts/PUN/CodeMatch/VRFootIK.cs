using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFootIK : MonoBehaviour
{
    [Range(0,1)]
    public float rightFootPosWeight = 1;
    [Range(0, 1)]
    public float leftFootPosWeight = 1;
    [Range(0, 1)]
    public float rightFootRotWeight = 1;
    [Range(0, 1)]
    public float leftFootRotWeight = 1;

    public Vector3 footOffset;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {        
        Vector3 rightFootPos = anim.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 leftFootPos = anim.GetIKPosition(AvatarIKGoal.LeftFoot);

        RaycastHit hit;
        bool hasHit = Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out hit); //right foot weight and hit detection

        if (hasHit)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootPosWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + footOffset);

            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal));
            anim.SetIKRotation(AvatarIKGoal.RightFoot, footRotation);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootRotWeight);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }        
       
        hasHit = Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out hit); //left foot weight and hit detection

        if (hasHit)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPosWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + footOffset);

            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal));
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, footRotation);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotWeight);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }
    }
}
