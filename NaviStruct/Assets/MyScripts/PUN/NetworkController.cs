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
        StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, 
            "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeMenu"));      
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

}
