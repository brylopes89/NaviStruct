using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScalar : MonoBehaviour
{
    public float defaultHeight = 2.5f;

    private void Start()
    {
       // ResizePlayerScale();
    }
    void Update()
    {
        //ResizePlayerScale();
    }

    public void ResizePlayerScale()
    {
        float headHeight = Camera.main.transform.localPosition.y;
        float scale = defaultHeight / headHeight;
        transform.localScale = new Vector3(1.5f,1.5f,1.5f) * scale;
    }
}
