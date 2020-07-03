using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlaygroundManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public void Awake()
    {
        if (MasterManager.ClassReference.Playground == null)
            MasterManager.ClassReference.Playground = this.gameObject;
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {        
        //GameSetupController setup = FindObjectOfType<GameSetupController>();
        //ChangeModeController modeChange = FindObjectOfType<ChangeModeController>();

        //setup.playground = this.gameObject;
        //modeChange.playground = this.gameObject;
    }
}
