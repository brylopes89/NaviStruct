using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerNames : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        nameText.text = pv.Owner.NickName;
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
