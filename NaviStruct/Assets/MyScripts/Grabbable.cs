using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    public Vector3 m_Offset = Vector3.zero;
    [HideInInspector]
    public HandManager m_ActiveHand = null;
    public float speed = 2f;
    private float maxSpeed = 15f;

    private Rigidbody rBody;
    private float moveScale;
    private Rigidbody[] childrenRB;

    private void Awake()
    {
        childrenRB = GetComponentsInChildren<Rigidbody>();
        rBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rBody.velocity.magnitude > maxSpeed)
        {
            rBody.velocity = Vector3.ClampMagnitude(rBody.velocity, maxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void SetKinematic(bool isKinematic)
    {
        rBody.isKinematic = isKinematic;
        //if (!isKinematic)
            //StartCoroutine(SetRigidBodyKinematic());
    }

    public IEnumerator SetRigidBodyKinematic()
    {
        yield return new WaitForSeconds(4f);
        rBody.isKinematic = true;
        
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
        Vector3 maxScale = new Vector3(5f, 5f, 5f);
        transform.localScale *= 1.02f;

        if (transform.localScale.x > 5f && transform.localScale.y > 5f && transform.localScale.z > 5f)
            transform.localScale = maxScale;
    }

    public void ScaleDown()
    {
        Vector3 minScale = new Vector3(.009f, .009f, .009f);
        transform.localScale /= 1.02f;

        if (transform.localScale.x < .009f && transform.localScale.y < .009f && transform.localScale.z < .009f)
            transform.localScale = minScale;
    }

    public void Rotate(bool isRotateRight)
    {
        if (isRotateRight)
            transform.Rotate(Vector3.up * speed);
        else
            transform.Rotate(-Vector3.up * speed);
    }
}
