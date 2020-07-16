using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChangeModeController : MonoBehaviourPunCallbacks 
{   
    [SerializeField]
    private GameObject dioramaFloor;
    [SerializeField]
    private GameObject playground;
    [SerializeField]
    private Button modeButton;
    
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float duration = 4f;
    [SerializeField]
    private float distance = 2f;

    [HideInInspector] 
    public bool isDiorama = false;

    private GameObject playerRig;

    private Vector3 targetWorldScale;
    private Vector3 originalWorldScale;
    private Vector3 currentPlayerPos;
    private Vector3 originalPlayerPos;
    private Vector3 targetPlayerPos;

    private Quaternion originalPlayerRot;   
    private Quaternion currentPlayerRot;
    private PhotonView pv;            

    private void Start()
    {
        playerRig = GameObject.Find("XR_Rig");
        pv = MasterManager.ClassReference.Avatar.GetComponent<PhotonView>();    

        originalPlayerPos = playerRig.transform.position;
        originalPlayerRot = playerRig.transform.rotation;
        originalWorldScale = playground.transform.localScale;
        
        targetWorldScale = new Vector3(0.01f, 0.01f, 0.01f);
        targetPlayerPos = playground.transform.forward * distance;
        
        modeButton.onClick.AddListener(() => ChangeMode(isDiorama));
    }

    private void ChangeMode(bool isChanging)
    {
        isChanging = !isChanging;
        isDiorama = isChanging;

        pv.RPC("ChangePlayerState", RpcTarget.All, PlayerStateManager.PlayerStates.Diorama, true);

        foreach (TextMeshProUGUI childText in modeButton.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (isChanging)
            {
                childText.text = "IMMERSIVE";
                DioramaPressed();
            }
            else
            {
                childText.text = "DIORAMA";
                ImmersivePressed();
            }
        }        
    }

    public void DioramaPressed()
    {     
        if (pv.IsMine)
        {
            dioramaFloor.SetActive(true);
            currentPlayerPos = playerRig.transform.position;
            currentPlayerRot = playerRig.transform.rotation;            

            StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, targetPlayerPos, duration));           
            StartCoroutine(ChangeWorldScale(playground.transform, originalWorldScale, targetWorldScale, duration));
        }        
    }
    
    public void ImmersivePressed()
    {
        if (pv.IsMine)
        {
            dioramaFloor.SetActive(false);
            currentPlayerPos = playerRig.transform.position;           

            StartCoroutine(ChangeWorldScale(playground.transform, targetWorldScale, originalWorldScale, duration));            
            StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, originalPlayerPos, duration));                   
        }                   
    }

    public void ResetPressed()
    {
        if (pv.IsMine)
        {
            currentPlayerPos = playerRig.transform.position;
            currentPlayerRot = playerRig.transform.rotation;
            if(isDiorama)
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, targetPlayerPos, duration));           
            else
                StartCoroutine(ChangePlayerPos(playerRig.transform, currentPlayerPos, originalPlayerPos, duration));
        }               
    }

    private IEnumerator ChangeWorldScale(Transform target, Vector3 startPos, Vector3 desiredPos, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;        

        while (i < 1)
        {            
            i += Time.deltaTime * rate;            
            target.localScale = Vector3.Lerp(startPos, desiredPos, i);                      

            yield return null;
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


}
