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

    private Vector3 targetScale;
    private Vector3 originalScale;
    private Vector3 originalPos;    
    private Quaternion originalRot;

    private float distance = 2.0f;
    private ObjectInteract interactable;
    
    
    // Start is called before the first frame update
    private void Start()
    {        
        interactable = stadium.GetComponent<ObjectInteract>();
        interactable.EnableCollider(false);        
        teleportFloor.SetActive(false);

        originalScale = stadium.transform.localScale;
        originalPos = stadium.transform.position;
        originalRot = stadium.transform.rotation;
        targetScale = new Vector3(0.004f, 0.004f, 0.004f);             
    }

    public void DioramaPressed()
    {               
        teleportFloor.SetActive(true);        

        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(currentPlayerPos.x, 0f, currentPlayerPos.z);        
        Vector3 targetPos = cam.transform.position + cam.transform.forward * distance;              

        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;       

        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));
        StartCoroutine(ChangeScale(currentScale, targetScale, duration));
        StartCoroutine(ChangePos(currentPos, targetPos, duration));

        //interactable.enabled = true;

        StartCoroutine(interactable.EnableCollider(true));
        
    }

    public void ImmersivePressed()
    {
        teleportFloor.SetActive(false);
        StartCoroutine(interactable.EnableCollider(false));       
        //interactable.enabled = false;

        Vector3 currentScale = stadium.transform.localScale;
        Vector3 currentPos = stadium.transform.position;
        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(0, 0, 0);
        Quaternion currentRotation = stadium.transform.rotation;
        
        StartCoroutine(ChangeScale(currentScale, originalScale, duration));
        StartCoroutine(ChangePos(currentPos, originalPos, duration));
        StartCoroutine(ChangeRotation(currentRotation, originalRot, duration));
        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));       
    }

    public void ResetPressed()
    {
        stadium.transform.position = cam.transform.position + cam.transform.forward * distance;
        stadium.transform.rotation = new Quaternion(0, 0, 0, 0);
        stadium.transform.localScale = targetScale;
    }

    private IEnumerator ChangeScale(Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangePos(Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangePlayerPos(Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangeRotation(Quaternion a, Quaternion b, float time)
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
