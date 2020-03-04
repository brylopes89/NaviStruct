using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteract : MonoBehaviour
{
    public Vector3 m_Offset = Vector3.zero;

    [HideInInspector]
    public HandManager m_ActiveHand = null;

    public virtual void Action()
    {
        print("Action");
    }   

    public void ApplyOffset(Transform parent)
    {
        transform.SetParent(parent);
        transform.localRotation = Quaternion.identity; //zeros out rotation
        transform.localPosition = m_Offset;
        transform.SetParent(null);
    }
}
