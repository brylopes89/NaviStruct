using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteract : MonoBehaviour
{
    public Vector3 m_Offset = Vector3.zero;
    [HideInInspector]
    public HandManager m_ActiveHand = null;    
    public float speed = 2f;
    public float maxSpeed = 3f;

    private Rigidbody rBody;
    private float moveScale;
    private MeshCollider[] childrenColliders;

    private void Awake()
    {
        childrenColliders = GetComponentsInChildren<MeshCollider>();
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
        if (collision.gameObject.CompareTag("Player"))
            SetKinematic(false);
    }

    public void ApplyOffset(Transform parent)
    {
        transform.SetParent(parent);
        transform.localRotation = Quaternion.identity; //zeros out rotation
        transform.localPosition = m_Offset;       
        transform.SetParent(null);
    }

    public IEnumerator EnableCollider(bool enableCol)
    {
        if (!enableCol)
        {
            GetComponent<BoxCollider>().enabled = false;
            foreach (MeshCollider col in childrenColliders)
                col.enabled = true;

            yield return null;
        }            

        else
        {
            foreach (MeshCollider col in childrenColliders)
                col.enabled = false;

            yield return new WaitForSeconds(2f);

            GetComponent<BoxCollider>().enabled = true;
            
        }        
    }

    public void SetKinematic(bool isKinematic)
    {
        rBody.isKinematic = isKinematic;

        if (!isKinematic)
            StartCoroutine(SetRigidBodyConstraints());
    }

    public IEnumerator SetRigidBodyConstraints()
    {       
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;     

        yield return new WaitForSeconds(4f);  
        
        rBody.isKinematic = true;
        rBody.constraints = RigidbodyConstraints.None;
    }

    public void SetMoveScale(Vector3 handPos)
    {
        Vector3 origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        moveScale = Vector3.Magnitude(transform.position - origin) / Vector3.Magnitude(handPos - origin);
    }

    public void Move(Vector3 curHandPos, Vector3 lastHandPos, Quaternion curHandRot, Quaternion lastHandRot)
    {
        rBody.MovePosition(rBody.position + (curHandPos - lastHandPos) * moveScale);           
        //rBody.MoveRotation(Quaternion.RotateTowards(lastHandRot, curHandRot, Time.deltaTime));
    } 

    public void ScaleUp()
    {
        Vector3 maxScale = new Vector3(0.01f, 0.01f, 0.01f);
        transform.localScale *= 1.02f;
        
        if (transform.localScale.x > .01f && transform.localScale.y > .01f && transform.localScale.z > 0.01f)
            transform.localScale = maxScale;
    }

    public void ScaleDown()
    {
        Vector3 minScale = new Vector3(.0009f, .0009f, .0009f);
        transform.localScale /= 1.02f;

        if (transform.localScale.x < .0009f && transform.localScale.y < .0009f && transform.localScale.z < .0009f)
            transform.localScale = minScale;
    }

    public void Rotate(bool isRotateRight)
    {
        if (isRotateRight)        
            transform.Rotate(Vector3.up * speed);
        else 
            transform.Rotate(-Vector3.up* speed);
    }
    
}
