using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaygroundManager : MonoBehaviour
{
    public float speed = 3f;

    public void Awake()
    {
        if (MasterManager.ClassReference.Playground == null)
            MasterManager.ClassReference.Playground = this.gameObject;
    }

    public void ApplyInitialScale(Vector3 worldScale)
    {
        this.transform.localScale = worldScale;        
    }

    public IEnumerator ChangeWorldScale(Vector3 startScale, Vector3 desiredScale, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            this.transform.localScale = Vector3.Lerp(startScale, desiredScale, i);

            yield return null;
        }
    }

}
