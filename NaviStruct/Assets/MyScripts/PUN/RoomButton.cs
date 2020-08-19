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
    private AnimationController animController;

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;
    }
    public void JoinRoomOnClick() //paired the button that is the room listing. 
    {
        if (PhotonNetwork.PlayerList.Length == roomSize)
        {
            StartCoroutine(animController.FadeStatusText("Room " + roomName + "is full. Please try another room."));
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        StartCoroutine(animController.FadeMenuPanels(animController.joinAnim, "IsFadeOut", animController.customRoomPanel, animController.joinPanel));
        StartCoroutine(animController.FadeStatusText("You have Joined Room " + roomName));
    }

    public void SetRoom(string nameInput, int sizeInput, int countInput)
    {
        roomName = nameInput;
        roomSize = sizeInput;
        playerCount = countInput;
        nameText.text = nameInput;
        sizeText.text = countInput + "/" + sizeInput;
    }
}
