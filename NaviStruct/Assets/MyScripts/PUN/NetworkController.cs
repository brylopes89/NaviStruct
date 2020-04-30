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
        //other ways to make a connection can be found here: https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()    
    {
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeMenu"));
        //AnimationController.animController.updateText.text = "We are now connected to the " + PhotonNetwork.CloudRegion + "server!";
        //Debug.Log("My nickname is " + PhotonNetwork.LocalPlayer.NickName);        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

}
