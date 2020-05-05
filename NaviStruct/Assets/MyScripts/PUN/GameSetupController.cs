using System.IO;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSetupController : MonoBehaviour
{
    public static GameSetupController gameSetup;

    [SerializeField]
    private int menuSceneIndex;
    public Transform[] spawnPoints;

    private int spawnPicker;

    private void OnEnable()
    {
        if (GameSetupController.gameSetup == null)
            GameSetupController.gameSetup = this;
    }

    private void Start()
    {
        spawnPicker = Random.Range(0, spawnPoints.Length);

        //CreatePlayer();
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player Kyle"),
            spawnPoints[spawnPicker].position, spawnPoints[spawnPicker].rotation, 0);
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
