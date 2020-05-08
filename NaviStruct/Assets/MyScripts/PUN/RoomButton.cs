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
        if (PhotonNetwork.PlayerList.Length == roomSize)
        {
            StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, "Room " + roomName + "is full. Please try another room.", "isFadeMenu"));
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        StartCoroutine(MenuAnimationController.animController.FadeAnimation(MenuAnimationController.animController.joinAnim, "IsFadeOut", MenuAnimationController.animController.roomPanel, MenuAnimationController.animController.joinPanel));
        StartCoroutine(MenuAnimationController.animController.ScreenTextFade(MenuAnimationController.animController.textAnim, "You have Joined Room " + roomName, "isFadeMenu"));
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
