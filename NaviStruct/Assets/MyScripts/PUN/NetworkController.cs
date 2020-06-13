using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;
using System.Collections;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private AnimationController animController;     
    
    void Start()
    {
        animController = MasterManager.ClassReference.AnimController;        

        XRSettings.enabled = false;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers        
    }
    
    public override void OnConnectedToMaster()    
    {        
        StartCoroutine(animController.FadeStatusText(animController.statusTextAnim, 
            "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeMenu"));     
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(animController.FadeStatusText(animController.statusTextAnim, "Joined Lobby", "isFadeMenu"));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(animController.FadeStatusText(animController.statusTextAnim, "Create Room Failed", "isFadeMenu"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(animController.FadeStatusText(animController.statusTextAnim, "Created Room Successfully", "isFadeMenu"));
    }
    
}
