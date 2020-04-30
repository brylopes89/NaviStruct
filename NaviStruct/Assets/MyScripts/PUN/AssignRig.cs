using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AssignRig : MonoBehaviour
{
    public static AssignRig assignRig;

    [SerializeField]
    private GameObject m_Head;
    public GameObject head { get { return m_Head; } set { m_Head = value; } }

    [SerializeField]
    private GameObject m_LeftHand;
    public GameObject leftHand { get { return m_LeftHand; } set { m_LeftHand = value; } }

    [SerializeField]
    private GameObject m_RightHand;
    public GameObject rightHand { get { return m_RightHand; } set { m_RightHand = value; } }     
}
