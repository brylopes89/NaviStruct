using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject startButton;

    private void Awake()
    {
        lobby = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Master Photon server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + "server!");
        startButton.SetActive(true);
    }

    public void OnStartButtonClick()
    {
        startButton.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
