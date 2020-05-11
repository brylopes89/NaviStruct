using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {                
        //PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        //PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers        
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()    
    {               
        MenuAnimationController.animController.statusText.text = "We are now connected to the " + PhotonNetwork.CloudRegion + "server!";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, "Joined Lobby", "isFadeMenu"));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, "Create Room Failed", "isFadeMenu"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, "Created Room Successfully", "isFadeMenu"));
    }
}
