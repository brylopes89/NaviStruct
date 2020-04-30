using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AddCamera : MonoBehaviour
{
    [HideInInspector]
    public Camera worldCamera;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
}
