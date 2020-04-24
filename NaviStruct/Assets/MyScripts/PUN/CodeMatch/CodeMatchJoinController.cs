using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeMatchJoinController : MonoBehaviourPunCallbacks
{
    [Header("Join Panel Display")]    
    [SerializeField]
    private TMP_InputField codeInputField;
    [SerializeField]
    private GameObject joinButton;
    private string joinCode;    

    [Header("Available Rooms Display")]
    [SerializeField]
    private Transform roomsContainer;
    [SerializeField]
    private GameObject roomListingPrefab;
    private List<RoomInfo> roomListings;

    // Start is called before the first frame update
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach (RoomInfo room in roomList)
        {
            if (roomListings != null) //try to find existing room listing
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            else
                tempIndex = -1;

            if (tempIndex != -1) //remove listing because it has been closed
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }

            if (room.PlayerCount > 0) //add room listing because it is new
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room) //Displays new room listing for the current room
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void CodeInput(string code)
    {
        joinCode = code;
    }

    public void JoinRoomOnClick()
    {
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.joinAnim, "IsFadeOut", AnimationController.animController.roomPanel, AnimationController.animController.joinPanel));        
        PhotonNetwork.JoinRoom(joinCode);
    }

    public void LeaveRoomOnClick()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.joinAnim, "IsFadeOut", AnimationController.animController.lobbyPanel, AnimationController.animController.joinPanel));        
    }

    public override void OnLeftRoom()
    {        
        codeInputField.text = "";
    }
}
