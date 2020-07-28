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
    private GameObject _ARRig;
    [SerializeField]
    private GameObject[] _ARChildObjects;
    [SerializeField]
    private GameObject _VRRig;
    [SerializeField]
    private GameObject _StandaloneRig;

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

#if UNITY_EDITOR && UNITY_ANDROID || UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID || UNITY_IOS
        avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_AR"),
                   Vector3.zero, Quaternion.identity, 0) as GameObject;       
        
#else               
        if (XRSettings.enabled)
        {            
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_VR"), 
                spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;       
        }
        else
        {
            avatarPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Avatars", "PlayerKyle_Stand"),
                spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;        
        }        
#endif
        MasterManager.ClassReference.Avatar = avatarPlayer;
    }

    private void SetXRActiveObjects(GameObject activeObject, GameObject deactiveObject1, GameObject deactiveObject2)
    {        
        pv = avatarPlayer.GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            activeObject.SetActive(true);
            deactiveObject1.SetActive(false);
            deactiveObject2.SetActive(false);

            for(int i = 0; i < _ARChildObjects.Length; i++)
            {              
                if(!_ARChildObjects[i].activeInHierarchy)
                    _ARChildObjects[i].SetActive(!_ARChildObjects[i].activeSelf);
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
