using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ChangeMode : MonoBehaviour
{
    public GameObject stadium;
    public GameObject teleportFloor;
    public Camera cam;
    public float speed = 4f;
    public float duration = 1f;

    private Vector3 originalScale;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private float distance = 2.0f;
    private bool isDiorama = false;
    
    // Start is called before the first frame update
    void Start()
    {
        originalScale = stadium.transform.localScale;
        originalPos = stadium.transform.position;
        originalRot = stadium.transform.rotation;
        teleportFloor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void DioramaPressed()
    {
        isDiorama = true;
        Vector3 targetScale = new Vector3(0.004f, 0.004f, 0.004f);
        Vector3 targetPos = cam.transform.position + cam.transform.forward * distance;
        Quaternion targetRot = new Quaternion(0, cam.transform.rotation.y, 0, cam.transform.rotation.w);

        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;
        Quaternion currentRot = stadium.transform.rotation;

        StartCoroutine(ChangeScale(currentScale, targetScale, duration));
        StartCoroutine(ChangePos(currentPos, targetPos, duration));

        teleportFloor.SetActive(true);
        //StartCoroutine(ChangeRotation(currentRot, targetRot, duration)); 
    }

    public void ImmersivePressed()
    {
        isDiorama = false;
        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;
        Quaternion currentRot = stadium.transform.rotation;
        
        StartCoroutine(ChangeScale(currentScale, originalScale, duration));
        StartCoroutine(ChangePos(currentPos, originalPos, duration));

        teleportFloor.SetActive(false);
        //StartCoroutine(ChangeRotation(currentRot, originalRot, duration)); 
    }

    public IEnumerator ChangeScale(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while(i < 1)
        {            
            i += Time.deltaTime * rate;            
            stadium.transform.localScale = Vector3.Lerp(a, b, i);

            yield return null;
        }        
    }

    public IEnumerator ChangePos(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            stadium.transform.position = Vector3.Lerp(a, b, i);

            yield return null;
        }
    }

    public IEnumerator ChangeRotation(Quaternion a, Quaternion b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            stadium.transform.rotation = Quaternion.Slerp(a, b, i);

            yield return null;
        }
    }
}
