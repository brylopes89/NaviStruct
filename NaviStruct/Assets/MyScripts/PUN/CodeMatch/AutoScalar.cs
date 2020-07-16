using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScalar : MonoBehaviour
{
    public float defaultHeight = 1.3f;

    void OnEnable()
    {        
        //ResizePlayerScale();
    }

    public void ResizePlayerScale()
    {
        float headHeight = Camera.main.transform.position.y;
        float scale = defaultHeight / headHeight;
        transform.localScale = Vector3.one * scale;
    }
}
