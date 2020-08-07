using System.Collections;
using UnityEngine;

public class RigTransformManager : MonoBehaviour
{    
    public float speed = 3f; 

    private void Awake()
    {
        if (MasterManager.ClassReference.PlayerRig == null)
            MasterManager.ClassReference.PlayerRig = this.gameObject;      
    }

    public IEnumerator ChangeRigPosition(Vector3 startPos, Vector3 desiredPos, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            this.transform.position = Vector3.Lerp(startPos, desiredPos, i);

            yield return null;
        }
    }
}
