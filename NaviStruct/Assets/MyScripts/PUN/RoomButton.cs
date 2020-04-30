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
        StartCoroutine(AnimationController.animController.FadeAnimation(AnimationController.animController.joinAnim, "IsFadeOut", AnimationController.animController.roomPanel, AnimationController.animController.joinPanel));
        StartCoroutine(AnimationController.animController.ScreenTextFade(AnimationController.animController.textAnim, "You have Joined Room " + roomName, "isFadeMenu"));
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
