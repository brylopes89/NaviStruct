using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CodeMatchRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject enterButton;
    [SerializeField]
    private TMP_Text playerCount;
    [SerializeField]
    private TMP_Text playerCount2;
    
    [SerializeField]
    private Transform scrollList;    
    [SerializeField]
    private GameObject playerListingPrefab; //instantiate to dispolay each player in the room

    public override void OnJoinedRoom() //called when the local player joins the room
    {
        enterButton.SetActive(false);
        playerCount.text = PhotonNetwork.PlayerList.Length + "Players: ";
        playerCount2.text = PhotonNetwork.PlayerList.Length + "Players: ";

        if (PhotonNetwork.IsMasterClient) //if master client then activate the start button
            enterButton.SetActive(true);
        else
            enterButton.SetActive(false);

        ClearPlayerListings();// remove all old player listings
        ListPlayers();// relist all current player listings
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListingPrefab, scrollList);
            TextMeshProUGUI tempText = tempListing.transform.GetChild(0).GetComponent< TextMeshProUGUI>();
            tempText.text = player.NickName;
        }
    }

    void ClearPlayerListings()
    {
        for (int i = scrollList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollList.GetChild(i).gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enters the room
    {
        playerCount.text = PhotonNetwork.PlayerList.Length + "Players: ";
        playerCount2.text = PhotonNetwork.PlayerList.Length + "Players: ";

        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a remote player leaves the room
    {
        playerCount.text = PhotonNetwork.PlayerList.Length + "Players: ";
        playerCount2.text = PhotonNetwork.PlayerList.Length + "Players: ";

        ClearPlayerListings();
        ListPlayers();

        if (PhotonNetwork.IsMasterClient)
            enterButton.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        playerCount.text = "Players: ";
        playerCount2.text = "Players: ";
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
