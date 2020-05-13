using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;
using UnityEditorInternal.VR;
using UnityEditor;
using Valve.VR;
using System.Collections;

public class NetworkController : MonoBehaviourPunCallbacks
{
    
    // Start is called before the first frame update
    void Start()
    {
        XRSettings.enabled = false;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectToBestCloudServer(); //Connects to Photon master servers        
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()    
    {
        //MenuAnimationController.animController.statusText.text = "We are now connected to the " + PhotonNetwork.CloudRegion + "server!";
        StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, 
            "We are now connected to the " + PhotonNetwork.CloudRegion + "server!", "isFadeStart"));     
    }

    public override void OnDisconnected(DisconnectCause cause)
    {        
        Debug.Log("Disconnected from server for reason: " + cause.ToString(), this);        
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "Joined Lobby", "isFadeMenu"));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "Create Room Failed", "isFadeMenu"));
    }

    public override void OnCreatedRoom()
    {
        StartCoroutine(AnimationController.instance.ScreenTextFade(AnimationController.instance.textAnim, "Created Room Successfully", "isFadeMenu"));
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
