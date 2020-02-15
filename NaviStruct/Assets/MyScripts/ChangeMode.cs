using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ChangeMode : MonoBehaviour
{
    public GameObject stadium;
    public GameObject teleportFloor;
    public GameObject player;
    public Camera cam;
    public float speed = 4f;
    public float duration = 1f;

    private Vector3 originalScale;
    private Vector3 originalPos;    
   //private Quaternion originalRot;

    private float distance = 2.0f;
   //private bool isDiorama = false;
    
    // Start is called before the first frame update
    void Start()
    {
        originalScale = stadium.transform.localScale;
        originalPos = stadium.transform.position;             

        teleportFloor.SetActive(false);
        stadium.GetComponent<ObjectSelector>().enabled = false;
    }

    public void DioramaPressed()
    {
        stadium.GetComponent<ObjectSelector>().enabled = true;

        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(currentPlayerPos.x, 0f, currentPlayerPos.z);
        Vector3 targetScale = new Vector3(0.004f, 0.004f, 0.004f);
        Vector3 targetPos = cam.transform.position + cam.transform.forward * distance;              

        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;       

        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));
        StartCoroutine(ChangeScale(currentScale, targetScale, duration));
        StartCoroutine(ChangePos(currentPos, targetPos, duration));        

        teleportFloor.SetActive(true);        
    }

    public void ImmersivePressed()
    {
        stadium.GetComponent<ObjectSelector>().enabled = false;

        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;
        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(0, 0, 0);
        
        
        StartCoroutine(ChangeScale(currentScale, originalScale, duration));
        StartCoroutine(ChangePos(currentPos, originalPos, duration));
        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));
        
        teleportFloor.SetActive(false);        
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

    public IEnumerator ChangePlayerPos(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            player.transform.position = Vector3.Lerp(a, b, i);

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
