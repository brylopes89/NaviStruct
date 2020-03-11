using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class AnchorManager : MonoBehaviour
{
    public GameObject anchoredPrefab;
    public GameObject unanchoredPrefab;

    private Anchor anchor;
    private Vector3 lastAnchoredPosition;
    private Quaternion lastAnchoredRotation;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            //anchor = Session.CreateAnchor(transform.position, transform.rotation);
            GameObject.Instantiate(anchoredPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform);
            GameObject.Instantiate(unanchoredPrefab, anchor.transform.position, anchor.transform.rotation);
            lastAnchoredPosition = anchor.transform.position;
            lastAnchoredRotation = anchor.transform.rotation;
        }
    }
}
