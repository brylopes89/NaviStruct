﻿using Photon.Pun;
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
        animController = SceneManagerSingleton.instance.animationController;
    }
    public void JoinRoomOnClick() //paired the button that is the room listing. 
    {
        if (PhotonNetwork.PlayerList.Length == roomSize)
        {
            StartCoroutine(animController.FadeText(animController.textAnim, "Room " + roomName + "is full. Please try another room.", "IsFadeMenu"));
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        StartCoroutine(animController.FadeAnimation(animController.joinAnim, "IsFadeOut", animController.roomPanel, animController.joinPanel));
        StartCoroutine(animController.FadeText(animController.textAnim, "You have Joined Room " + roomName, "IsFadeMenu"));
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
