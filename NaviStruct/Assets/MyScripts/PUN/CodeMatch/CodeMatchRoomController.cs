using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class CodeMatchRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject enterButton;
    [SerializeField]
    private TMP_Text playerCount;
    [SerializeField]
    private TMP_Text playerCount2;

    public override void OnJoinedRoom() //called when the local player joins the room
    {
        enterButton.SetActive(false);
        playerCount.text = PhotonNetwork.PlayerList.Length + " Players";
        playerCount2.text = PhotonNetwork.PlayerList.Length + " Players";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enters the room
    {
        playerCount.text = PhotonNetwork.PlayerList.Length + " Players";
        playerCount2.text = PhotonNetwork.PlayerList.Length + " Players";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a remote player leaves the room
    {
        playerCount.text = PhotonNetwork.PlayerList.Length + " Players";
        playerCount2.text = PhotonNetwork.PlayerList.Length + " Players";
    }

    public override void OnLeftRoom()
    {
        playerCount.text = "0 Players";
        playerCount2.text = "0 Players";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public void StartGameOnClick()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
