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
        StartCoroutine(animController.FadeText(animController.textAnim, 
            "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeMenu"));     
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(animController.FadeText(animController.textAnim, "Joined Lobby", "isFadeMenu"));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(animController.FadeText(animController.textAnim, "Create Room Failed", "isFadeMenu"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(animController.FadeText(animController.textAnim, "Created Room Successfully", "isFadeMenu"));
    }
    
}
