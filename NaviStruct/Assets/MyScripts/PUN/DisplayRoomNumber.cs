using UnityEngine;
using TMPro;

public class DisplayRoomNumber : MonoBehaviour
{    
    private TextMeshProUGUI roomText;
    // Start is called before the first frame update
    void Awake()
    {
        roomText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        //roomText.text = "Room #: " + DelayStartLobbyController.randomRoomNumber;
    }
}
