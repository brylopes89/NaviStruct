using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.Playables;
using Photon.Realtime;

public class GameSetupController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int menuSceneIndex;   
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private GameObject[] arComponents;
    [SerializeField]
    private GameObject[] playerRigs;

    [HideInInspector]
    public GameObject avatarPlayer;  
    
    private int spawnPicker;
    private PhotonView pv;

    private void Awake()
    {
        if (MasterManager.ClassReference.GameSetupController == null)
            MasterManager.ClassReference.GameSetupController = this;
        
        CreatePlayer();               
    }
    
    private void CreatePlayer()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        if (XRSettings.enabled)
        {
            if(MasterManager.ClassReference.IsVRSupport)
                avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_VR"), 
                    spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;       
            else
                avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_AR"),
                    spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_Stand"),
                spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;           
        }
        
        MasterManager.ClassReference.Avatar = avatarPlayer;
        SetXRComponents();        
    }

    private void SetXRComponents()
    {
        pv = avatarPlayer.GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            foreach(GameObject component in arComponents)
            {
                if (XRSettings.enabled && MasterManager.ClassReference.IsVRSupport || !XRSettings.enabled)
                {
                    component.SetActive(false);
                }
                else
                {
                    component.SetActive(true);
                    playerRigs[playerRigs.Length].SetActive(false);
                }
            }                
        }
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;

        SceneManager.LoadScene(menuSceneIndex);
    }
}
