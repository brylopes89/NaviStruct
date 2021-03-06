﻿using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PhysicsPointer : MonoBehaviour
{
    public float defaultLength = 3.0f;
    public GameObject dot;    

    private LineRenderer lineRenderer = null;
    private HandManager hand;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        hand = GetComponentInParent<HandManager>();
    }

    private void Update()
    {
        UpdateLength();            
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, CalculateEnd());

        dot.transform.position = CalculateEnd();
    }

    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if (hit.collider)
            endPosition = hit.point;          

        return endPosition;
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }
}
