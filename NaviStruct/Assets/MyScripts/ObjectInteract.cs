using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteract : MonoBehaviour
{
    public Vector3 m_Offset = Vector3.zero;
    [HideInInspector]
    public HandManager m_ActiveHand = null;

    private Rigidbody rBody;
    private float moveScale;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
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

    public void Grab(bool shouldGrab)
    {
        rBody.isKinematic = shouldGrab;
    }

    public void SetMoveScale(Vector3 handPos)
    {
        Vector3 origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        moveScale = Vector3.Magnitude(transform.position - origin) / Vector3.Magnitude(handPos - origin);
    }

    public void Move(Vector3 curHandPos, Vector3 lastHandPos, Quaternion curHandRot, Quaternion lastHandRot)
    {
        rBody.MovePosition(rBody.position + (curHandPos - lastHandPos) * moveScale);           
        rBody.MoveRotation(Quaternion.RotateTowards(lastHandRot, curHandRot, Time.deltaTime));
    } 

    public void ScaleUp()
    {
        Vector3 maxScale = new Vector3(2f, 2f, 2f);
        transform.localScale *= 1.02f;
        
        if (transform.localScale.x > 2f && transform.localScale.y > 2f && transform.localScale.z > 2f)
            transform.localScale = maxScale;
    }

    public void ScaleDown()
    {
        Vector3 minScale = new Vector3(.008f, .008f, .008f);
        transform.localScale /= 1.02f;

        if (transform.localScale.x < .008f && transform.localScale.y < .008f && transform.localScale.z < .008f)
            transform.localScale = minScale;
    }
}
