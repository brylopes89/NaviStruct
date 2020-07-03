using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChangeModeController : MonoBehaviourPunCallbacks 
{   
    public GameObject dioramaFloor;    
    public GameObject playground;
    public Transform playerRig;
    public Button modeButton;

    [SerializeField]
    private float stateChangeWaitTime;
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float duration = 4f;
    [SerializeField]
    private float resetDuration = .5f;
    [SerializeField]
    private float distance = 2f;

    [HideInInspector] public bool isDiorama = false;

    private Transform parentRig;
    private GameObject avatar;

    private Vector3 targetWorldScale;
    private Vector3 originalWorldScale;
    private Vector3 currentPlayerPos;
    private Vector3 originalPlayerPos;
    private Vector3 targetPlayerPos;

    private Quaternion originalPlayerRot;   
    private Quaternion currentPlayerRot;
    private PhotonView pv;    

    private float stateChangeTimer = 3f;
    public bool isStateChange;

    // Start is called before the first frame update
    private void Start()
    {
        stateChangeTimer = stateChangeWaitTime;

        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();        
        parentRig = MasterManager.ClassReference.Avatar.transform.parent;
        avatar = MasterManager.ClassReference.Avatar;

        originalPlayerPos = MasterManager.ClassReference.Avatar.transform.position;
        originalPlayerRot = MasterManager.ClassReference.Avatar.transform.rotation;
        originalWorldScale = playground.transform.localScale;
        
        targetWorldScale = new Vector3(0.008f, 0.008f, 0.008f);
        targetPlayerPos = (playground.transform.forward + Vector3.up) * distance;
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void DioramaPressed()
    {       
        isDiorama = true;
        isStateChange = true;

        //modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "IMMERSIVE";
        if (pv.IsMine)
        {
            currentPlayerPos = playerRig.transform.position;
            currentPlayerRot = playerRig.transform.rotation;
            //dioramaFloor.SetActive(true);

            StartCoroutine(ChangePlayerPos(playerRig, currentPlayerPos, targetPlayerPos, duration));
            StartCoroutine(ChangePlayerRot(currentPlayerRot, originalPlayerRot, duration, playerRig.rotation));
            StartCoroutine(ChangeWorldScale(originalWorldScale, targetWorldScale, duration));
        }       
    }

    public void ImmersivePressed()
    {
        isDiorama = false;
        isStateChange = true;

        if (pv.IsMine)
        {                       
            currentPlayerPos = playerRig.transform.position;
            //dioramaFloor.SetActive(false);

            StartCoroutine(ChangeWorldScale(targetWorldScale, originalWorldScale, duration));
            StartCoroutine(ChangePlayerPos(playerRig, currentPlayerPos, originalPlayerPos, duration));                   
        }                   
    }

    public void ResetPressed()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = playerRig.transform.position;
            currentPlayerRot = playerRig.transform.rotation;
            if(isDiorama)
                StartCoroutine(ChangePlayerPos(playerRig, currentPlayerPos, originalPlayerPos, duration));
            else
                StartCoroutine(ChangePlayerPos(playerRig, currentPlayerPos, targetPlayerPos, duration));
            //StartCoroutine(ChangePlayerRot(currentPlayerRot, originalPlayerRot, duration, playerRig.rotation));
        }               
    }

    private IEnumerator ChangeWorldScale(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;        

        if (isDiorama)
        {
            parentRig.transform.SetParent(null);
            yield return new WaitForFixedUpdate();
        }            

        while (i < 1)
        {            
            i += Time.deltaTime * rate;            
            playground.transform.localScale = Vector3.Lerp(a, b, i);           

            yield return null;
        }

        if (!isDiorama)
        {
            yield return new WaitForSeconds(.2f);
            parentRig.transform.SetParent(playground.transform);
        }        
    }

    private IEnumerator ChangePlayerPos(Transform target, Vector3 a, Vector3 b, float time)
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

    private IEnumerator ChangePlayerRot(Quaternion a, Quaternion b, float time, Quaternion objectRotation)
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

    private void UpdateTimer()
    {
        if (isStateChange)
        {
            stateChangeTimer -= Time.deltaTime;            

            if (stateChangeTimer <= 0)
            {
                stateChangeTimer = stateChangeWaitTime;
                isStateChange = false;
            }

            pv.RPC("StateChangeBegin", RpcTarget.All, isStateChange);
        }        
    }
}
