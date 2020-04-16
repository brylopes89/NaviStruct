using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class CustomMatchRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private GameObject roomPanel;

    [SerializeField]
    private GameObject startButton;

    [SerializeField]
    private Transform playersContainer; //used to display all the players in the current room
    [SerializeField]
    private GameObject playerListingPrefab; //instantiate to dispolay each player in the room

    [SerializeField]
    private TextMeshProUGUI roomNameDisplay;

    void ClearPlayerListings()
    {
        for(int i = playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tempText.text = player.NickName;
        }
    }

    public override void OnJoinedRoom() //called when the local player joins the room
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name; //update room name display

        if (PhotonNetwork.IsMasterClient) //if master client then activate the start button
            startButton.SetActive(true);
        else
            startButton.SetActive(false);

        ClearPlayerListings();// remove all old player listings
        ListPlayers();// relist all current player listings
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enters the room
    {
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a player leaves the room
    {
        ClearPlayerListings();
        ListPlayers();

        if (PhotonNetwork.IsMasterClient)
            startButton.SetActive(true);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }
}
