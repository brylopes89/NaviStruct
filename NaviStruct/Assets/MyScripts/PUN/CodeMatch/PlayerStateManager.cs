using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;

public enum PlayerStates { Immersive = 0, Diorama = 1}

public class PlayerStateManager : MonoBehaviourPun
{
    private bool isStateChange = false;
    public bool IsStateChange { get { return isStateChange; } private set { isStateChange = value; } }   
    public PlayerStates currentState { get; private set; }

    [SerializeField]
    private float stateChangeWaitTime;

    private float stateChangeTimer;
    private PlayerAvatarManager avatar_Manager;    

    private void Awake()
    {
        stateChangeTimer = stateChangeWaitTime;
        avatar_Manager = GetComponent<PlayerAvatarManager>();             
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (IsStateChange)
        {
            stateChangeTimer -= Time.deltaTime;

            if (stateChangeTimer <= 0)
            {                
                stateChangeTimer = stateChangeWaitTime;
                SetNextState(false);                
            }
        }
    }

    public void SetPlayerState(PlayerStates state)
    {          
        if(photonView.IsMine)
            this.currentState = state;        
    }

    public void SetNextState(bool isChange)
    {
        IsStateChange = isChange;
        var length = System.Enum.GetValues(typeof(PlayerStates)).Length;

        if (IsStateChange && photonView.IsMine)        
            this.currentState = (PlayerStates)(((int)currentState + 1) % length);                      

        avatar_Manager.SetAvatarParent(isStateChange);
    }
}
