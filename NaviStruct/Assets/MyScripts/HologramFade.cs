using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramFade : MonoBehaviour
{
    Material holoMat;
    public float targetValue = 0;
    public float currentValue = 1;
    public float duration = 1;
    public float speed = 1.5f;

    // Start is called before the first frame update
    void Awake()
    {
        holoMat = GetComponent<Renderer>().material;
        //holoMat.SetFloat("_CutoutThresh", currentValue);
        StartCoroutine(ChangeValue());

    }

    void Update()
    {
        
    }

    private IEnumerator ChangeValue()
    {
        float i = 0.0f;
        float rate = (1.0f / duration) * speed;

        while (i < duration)
        {
            i += Time.deltaTime * rate;
            //currentValue = Mathf.Lerp(currentValue, targetValue, i);
            holoMat.SetFloat("_CutoutThresh", Mathf.Lerp(currentValue, targetValue, i));
            yield return null;
        }
    }
}
