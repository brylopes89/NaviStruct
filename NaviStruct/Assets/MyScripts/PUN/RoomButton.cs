using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI sizeText;

    private string roomName;
    private int roomSize;
    private int playerCount;

    public void JoinRoomOnClick() //paired the button that is the room listing. 
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void SetRoom(string nameInput, int sizeInput, int countInput)
    {
        roomName = nameInput;
        roomSize = sizeInput;
        playerCount = countInput;
        nameText.text = nameInput;
        sizeText.text = sizeInput + "/" + sizeInput;
    }
}
