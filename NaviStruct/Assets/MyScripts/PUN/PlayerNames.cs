using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerNames : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;   

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
