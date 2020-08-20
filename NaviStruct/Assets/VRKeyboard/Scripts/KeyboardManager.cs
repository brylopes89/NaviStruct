/***
 * Author: Yunhan Li
 * Any issue please contact yunhn.lee@gmail.com
 ***/

using UnityEngine;
using TMPro;
using System.Reflection;

namespace VRKeyboard.Utils
{
    public class KeyboardManager : MonoBehaviour
    {        
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = true;        
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

        private string inputFieldName;
        private Key[] keyList;        
        private bool capslockFlag;
        private bool secondFlag;
        private int flagCount = 0;
        private CodeMatchLobbyController lobbyController;        
       
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
            
            if (capslockFlag)
            {                
                flagCount = 1;
            }                

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
                ChangeInputFieldValue();
            }
            else
            {
                return;
            }
        }

        public void ClearOnDeselect()
        {
            Input = "";

            capslockFlag = true;
            flagCount = 1;

            CapsLock();
        }

        public void ClearInputText()
        {
            Input = "";
            ChangeInputFieldValue();
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

            flagCount += 1;

            if (flagCount > 2)
                flagCount = 0;

            switch (flagCount)
            {
                case 1:
                    capslockFlag = true;
                    secondFlag = false;
                    break;

                case 2:
                    capslockFlag = true;
                    secondFlag = true;
                    break;

                default:                    
                    capslockFlag = false;
                    secondFlag = false;
                    break;
            }            

            Debug.Log(flagCount);
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

            if (flagCount == 2 && Input.Length >= 0)
            {                
                capslockFlag = false;

                CapsLock();
            }

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