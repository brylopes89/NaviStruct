using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScalar : MonoBehaviour
{
    public float defaultHeight = 2.5f;

    private void OnEnable()
    {
        ResizePlayerScale();
    }
    void Update()
    {
        //ResizePlayerScale();
    }

    public void ResizePlayerScale()
    {
        float headHeight = Camera.main.transform.localPosition.y;
        float scale = defaultHeight / headHeight;
        transform.localScale = Vector3.one * scale;
    }
}
