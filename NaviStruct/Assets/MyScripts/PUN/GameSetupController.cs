using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameSetupController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public GameObject avatarPlayer;

    private int menuSceneIndex = 0;
    private int spawnPicker;    

    private GameObject _Playground;
    private Transform[] spawnPoints;

    private void Awake()
    {
        if (MasterManager.ClassReference.GameSetupController == null)
            MasterManager.ClassReference.GameSetupController = this;

        CreatePlayground();
        CreatePlayer();               
    }
    
    private void CreatePlayground()
    {
        Vector3 desiredPos = new Vector3(0f, 1.5f, 0f);
        _Playground = PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs/SceneObjects", "Playground_2"), desiredPos, Quaternion.identity);

        GetSpawnPoints();
    }

    private void GetSpawnPoints()
    {
        Transform spawnPoint = _Playground.transform.GetChild(0);
        spawnPoints = spawnPoint.GetComponentsInChildren<Transform>();
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
