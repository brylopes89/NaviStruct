using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerStateManager : MonoBehaviourPunCallbacks
{
    public MonoBehaviour[] localScripts;
    public GameObject[] localObjects;

    private GameObject xrRig;
    private GameObject standaloneRig;
    private GameObject playground;

    private Transform parentRig;
    private PhotonTransformView proxyPV;     

    private void Awake()
    {
        parentRig = transform.parent;
        playground = GameObject.Find("Playground");
        proxyPV = GameObject.Find("Remote_Proxy").GetComponent<PhotonTransformView>();        
    }
    void Start()
    {
        parentRig.transform.SetParent(playground.transform);
        if (!photonView.IsMine)        
        {           
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
    }

    [PunRPC]
    public void StateChangeBegin(bool isStateChange)
    {
        if (isStateChange)
        {           
            GetComponent<PhotonTransformView>().m_SynchronizePosition = false;
            GetComponent<PhotonTransformView>().m_SynchronizeRotation = false;
            GetComponent<PhotonTransformView>().m_SynchronizeScale = false;               
        }
        else
        {            
            GetComponent<PhotonTransformView>().m_SynchronizePosition = true;
            GetComponent<PhotonTransformView>().m_SynchronizeRotation = true;
            GetComponent<PhotonTransformView>().m_SynchronizeScale = true;
        }
        
    }
}
