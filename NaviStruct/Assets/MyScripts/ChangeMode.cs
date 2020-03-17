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

    [HideInInspector] public bool isDiorama = false;       

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
        teleportFloor.SetActive(false);

        originalScale = stadium.transform.localScale;
        originalPos = stadium.transform.position;
        originalRot = stadium.transform.rotation;
        targetScale = new Vector3(0.004f, 0.004f, 0.004f);             
    }

    public void DioramaPressed()
    {
        isDiorama = true;
        teleportFloor.SetActive(true);        

        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(currentPlayerPos.x, 0f, currentPlayerPos.z);
        
        Vector3 targetStadiumPos = cam.transform.position + cam.transform.forward * distance;        
        Vector3 curStadiumScale = stadium.transform.localScale;
        Vector3 curStadiumPos = stadium.transform.position;       

        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));
        StartCoroutine(ChangeStadiumScale(curStadiumScale, targetScale, duration));
        StartCoroutine(ChangeStadiumPos(curStadiumPos, targetStadiumPos, duration));             
    }

    public void ImmersivePressed()
    {
        isDiorama = false;
        teleportFloor.SetActive(false);        

        Vector3 currentPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = new Vector3(0, 0, 0);

        Vector3 curStadiumScale = stadium.transform.localScale;
        Vector3 curStadiumPos = stadium.transform.position;
        Quaternion curStadiumRot = stadium.transform.rotation;
        
        StartCoroutine(ChangeStadiumScale(curStadiumScale, originalScale, duration));
        StartCoroutine(ChangeStadiumPos(curStadiumPos, originalPos, duration));
        StartCoroutine(ChangeStadiumRot(curStadiumRot, originalRot, duration));
        StartCoroutine(ChangePlayerPos(currentPlayerPos, targetPlayerPos, duration));       
    }

    public void ResetPressed()
    {
        stadium.transform.position = cam.transform.position + cam.transform.forward * distance;
        stadium.transform.rotation = new Quaternion(0, 0, 0, 0);
        stadium.transform.localScale = targetScale;
    }

    private IEnumerator ChangeStadiumScale(Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangeStadiumPos(Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangeStadiumRot(Quaternion a, Quaternion b, float time)
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
