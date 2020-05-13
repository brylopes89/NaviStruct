using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSetupController : MonoBehaviour
{
    //public static GameSetupController gameSetup;

    [SerializeField]
    private int menuSceneIndex;    
    [HideInInspector]
    public GameObject avatarPrefab;

    private int spawnPicker;

    private void Awake()
    {
       //if (GameSetupController.gameSetup == null)
            //GameSetupController.gameSetup = this;
    }

    private void Start()
    {        
        //CreatePlayer();
    }

    /*private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        avatarPrefab = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player Kyle"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0) as GameObject;         
    }*/

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
