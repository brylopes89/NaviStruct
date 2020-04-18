using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StadiumGrab : MonoBehaviour
{
    [SerializeField]
    private ChangeMode changeMode;
    private MeshCollider[] childrenColliders;
    private Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        childrenColliders = GetComponentsInChildren<MeshCollider>();
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator EnableCollider(bool enableCol)
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

            yield return new WaitForSeconds(1f);

            GetComponent<BoxCollider>().enabled = true;
        }
    }
}
