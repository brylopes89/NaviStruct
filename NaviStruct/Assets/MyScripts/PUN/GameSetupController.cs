using System.IO;
using UnityEngine;
using Photon.Pun;

public class GameSetupController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "XRPlayer_PUN"), Vector3.zero, Quaternion.identity);        
    }
}
