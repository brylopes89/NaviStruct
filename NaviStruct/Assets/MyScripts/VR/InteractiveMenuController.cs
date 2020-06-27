using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveMenuController : MonoBehaviour
{
    public GameObject stadium;
    public GameObject teleportFloor;    
    public Transform playground;
    public Transform playerRig;
    //private Camera cam;

    public float speed = 3f;
    public float duration = 4f;
    public float resetDuration = .5f;

    [HideInInspector] public bool isDiorama = false;       

    private Vector3 targetWorldScale;
    private Vector3 originalWorldScale;
    private Vector3 currentPlayerPos;
    private Vector3 originalPlayerPos;
    private Vector3 targetPlayerPos;

    private Quaternion originalPlayerRot;   
    private Quaternion currentPlayerRot;    

    // Start is called before the first frame update
    private void Start()
    {
        originalPlayerPos = playerRig.position;
        originalPlayerRot = playerRig.rotation;            
        originalWorldScale = playground.localScale;

        targetWorldScale = new Vector3(0.006f, 0.006f, 0.006f);        
        targetPlayerPos = Vector3.zero;
    }

    public void DioramaPressed()
    {
        isDiorama = true;     
        currentPlayerPos = playerRig.transform.position;
        currentPlayerRot = playerRig.transform.rotation;

        StartCoroutine(ChangePosition(playerRig, currentPlayerPos, targetPlayerPos, duration));
        StartCoroutine(ChangePlayerRot(currentPlayerRot, originalPlayerRot, duration, playerRig.rotation));        
        StartCoroutine(ChangeWorldScale(originalWorldScale, targetWorldScale, duration));                    
    }

    public void ImmersivePressed()
    {
        isDiorama = false;
        currentPlayerPos = playerRig.transform.position;

        StartCoroutine(ChangePosition(playerRig, currentPlayerPos, originalPlayerPos, duration));
        //StartCoroutine(ChangePosition(playground, targetWorldPos, originalWorldPos, duration));
        StartCoroutine(ChangeWorldScale(targetWorldScale, originalWorldScale, duration));                
    }

    public void ResetPressed()
    {        
        currentPlayerPos = playerRig.transform.position;
        currentPlayerRot = playerRig.transform.rotation;

        StartCoroutine(ChangePosition(playerRig, currentPlayerPos, targetPlayerPos, duration));
        StartCoroutine(ChangePlayerRot(currentPlayerRot, originalPlayerRot, duration, playerRig.rotation));
    }

    private IEnumerator ChangeWorldScale(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while(i < 1)
        {            
            i += Time.deltaTime * rate;            
            playground.transform.localScale = Vector3.Lerp(a, b, i);

            yield return null;
        }        
    }

    private IEnumerator ChangePosition(Transform target, Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            target.position = Vector3.Lerp(a, b, i);

            yield return null;
        }
    }

    public IEnumerator ChangePlayerRot(Quaternion a, Quaternion b, float time, Quaternion objectRotation)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            objectRotation = Quaternion.Slerp(a, b, i);

            yield return null;
        }
    }

}
