using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;
using System.Collections;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private AnimationController animController;
    // Start is called before the first frame update
    void Start()
    {
        animController = SceneManagerSingleton.instance.animationController;
        XRSettings.enabled = false;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers        
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()    
    {        
        StartCoroutine(animController.ScreenTextFade(animController.textAnim, 
            "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeStart"));     
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(animController.ScreenTextFade(animController.textAnim, "Joined Lobby", "isFadeMenu"));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(animController.ScreenTextFade(animController.textAnim, "Create Room Failed", "isFadeMenu"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(animController.ScreenTextFade(animController.textAnim, "Created Room Successfully", "isFadeMenu"));
    }

    public void VRToggleOnClick(bool isToggle)
    {        
        if(isToggle)
            StartCoroutine(EnableVRSupport(isToggle));              
    }

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();

        if (activateDevice)
        {
            XRSettings.LoadDeviceByName("OpenVR");
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = true;
        }
        else
        {
            XRSettings.LoadDeviceByName("None");
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;
        }   
    }
}
