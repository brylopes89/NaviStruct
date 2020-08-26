using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerNames : MonoBehaviourPun
{
    [SerializeField]
    private TextMeshProUGUI nameText;    

    // Start is called before the first frame update
    void Awake()
    {         
        nameText.text = this.photonView.Owner.NickName; 
    }
}
