using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class PlayerAvatarManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private MonoBehaviour[] localScripts;
    [SerializeField]
    private GameObject[] localObjects;

    [SerializeField]
    private GameObject avatarMesh;
    [SerializeField]
    private GameObject[] handPrefabs;
    [SerializeField]
    private float stateChangeWaitTime;    

    private float stateChangeTimer;

    private bool isStateChange = false;
    private bool isVREnabled = false;

    private GameObject playground;
    private XRController[] controllers;       

    private void Awake()
    {
        playground = GameObject.Find("Playground");
        controllers = GameObject.Find("XR_Rig").GetComponentsInChildren<XRController>();
        isVREnabled = MasterManager.ClassReference.IsVRSupport;

        stateChangeTimer = stateChangeWaitTime;     
    }

    void Start()
    {
        this.transform.SetParent(playground.transform);        

        if (!photonView.IsMine)        
        {            
            if (avatarMesh != null && isVREnabled)
            {
                avatarMesh.SetActive(true);
                for(int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].modelPrefab = null;
                }                
            }                
           
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
        else
        {
            if (avatarMesh != null && isVREnabled)
            {
                avatarMesh.SetActive(false);
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].modelPrefab = handPrefabs[i].transform;
                }
            }                    
        }
    }

    private void Update()
    {
        UpdateTimer();       
    }

    private void UpdateTimer()
    {
        if (isStateChange)
        {
            stateChangeTimer -= Time.deltaTime;

            if (stateChangeTimer <= 0)
            {                
                SetAvatarParent(false);
                stateChangeTimer = stateChangeWaitTime;                     
            }
        }
    }   
    
    public void SetAvatarParent(bool isChanging)
    {       
        isStateChange = isChanging;

        if (photonView.IsMine)
        {
            if (isChanging)
                this.transform.SetParent(null);
            else
                this.transform.SetParent(playground.transform);
        }                      
    }   
}
