/***
 * Author: Yunhan Li
 * Any issue please contact yunhn.lee@gmail.com
 ***/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Chat.UtilityScripts;
using System.Collections;

namespace VRKeyboard.Utils
{
    public class KeyboardManager : MonoBehaviour
    {
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = false;
        public int maxInputLength;

        [Header("UI Elements")]
        public TextMeshProUGUI inputText;

        [Header("Essentials")]
        public Transform keys;
        #endregion
        
        #region Private Variables
        private string Input
        {
            get { return inputText.text; }
            set { inputText.text = value; }
        }
        private Key[] keyList;        
        private bool capslockFlag;
        private CodeMatchLobbyController lobbyController;
        private string inputFieldName;
        #endregion

        #region Monobehaviour Callbacks
        void Awake()
        {
            keyList = keys.GetComponentsInChildren<Key>();

            if (MasterManager.ClassReference.KeyboardManager == null)
                MasterManager.ClassReference.KeyboardManager = this;
        }
        
        void Start()
        {
            lobbyController = MasterManager.ClassReference.LobbyController;

            foreach (var key in keyList)
            {
                key.OnKeyClicked += GenerateInput;                    
            }
            capslockFlag = isUppercase;
            CapsLock();            
        }

        

        private void OnDestroy()
        {
            foreach(var key in keyList)
            {
                key.OnKeyClicked -= GenerateInput;                
            }
        }

        #endregion

        #region Public Methods
        public void Backspace()
        {
            if (Input.Length > 0)
            {
                Input = Input.Remove(Input.Length - 1);
            }
            else
            {
                return;
            }
        }

        public void Clear()
        {
            Input = "";
            lobbyController.inputFieldDisplayText = Input;
        }

        public void CapsLock()
        {
            foreach (var key in keyList)
            {
                if (key is Alphabet)
                {
                    key.CapsLock(capslockFlag);
                }
            }
            capslockFlag = !capslockFlag;
        }

        public void Shift()
        {
            foreach (var key in keyList)
            {
                if (key is Shift)
                {
                    key.ShiftKey();
                }
            }
        }

        public void GenerateInput(string s)
        {
            if (Input.Length > maxInputLength) { return; }
            Input += s;            

            ChangeInputFieldValue();
        }

        private void ChangeInputFieldValue()
        {
            switch (inputFieldName)
            {
                case "CodeInput":
                    lobbyController.CodeInput(Input);
                    break;
                case "RoomNameInput":
                    lobbyController.OnRoomNameInput(Input);
                    break;
                case "RoomSizeInput":
                    lobbyController.OnRoomSizeInput(Input);
                    break;
                default:
                    lobbyController.OnPlayerNameInput(Input);
                    break;
            }
        }

        public void SelectedInputFieldName(string name)
        {
            inputFieldName = name;
        }
        #endregion
    }
}