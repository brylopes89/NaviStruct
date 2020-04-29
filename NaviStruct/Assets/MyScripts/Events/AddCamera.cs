using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

}
