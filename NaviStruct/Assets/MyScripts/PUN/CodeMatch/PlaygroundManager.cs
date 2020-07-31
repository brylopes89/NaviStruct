using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaygroundManager : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    public void Awake()
    {
        if (MasterManager.ClassReference.Playground == null)
            MasterManager.ClassReference.Playground = this.gameObject;
    }

    public void ApplyTransform(Vector3 worldScale)
    {
        this.transform.localScale = worldScale;        
    }

    public IEnumerator ChangeWorldScale(Vector3 startPos, Vector3 desiredPos, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            this.transform.localScale = Vector3.Lerp(startPos, desiredPos, i);

            yield return null;
        }
    }

}
