using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using VRKeyboard.Utils;

namespace Michsky.UI.ModernUIPack
{
    public class CustomInputField : MonoBehaviour, IPointerClickHandler, ISelectHandler
    {
        [Header("RESOURCES")]
        public GameObject fieldTrigger;
        private TMP_InputField inputText;
        private Animator inputFieldAnimator;
        public KeyboardManager keyboardManager;

        // [Header("SETTINGS")]
        private bool isEmpty = true;
        private bool isClicked = false;
        private string inAnim = "In";
        private string outAnim = "Out";        

        void Start()
        {            
            inputFieldAnimator = gameObject.GetComponent<Animator>();
            inputText = gameObject.GetComponent<TMP_InputField>();            

            // Check if text is empty or not
            if (inputText.text.Length == 0 || inputText.text.Length <= 0)
                isEmpty = true;

            else
                isEmpty = false;

            // Animate if it's empty
            if (isEmpty == true)
                inputFieldAnimator.Play(outAnim);

            else
                inputFieldAnimator.Play(inAnim);
        }

        void Update()
        {
            if (inputText.text.Length == 1 || inputText.text.Length >= 1)
            {
                isEmpty = false;
                inputFieldAnimator.Play(inAnim);
            }

            else if (isClicked == false)
            {
                inputFieldAnimator.Play(outAnim);
            }
        }

        public void Animate()
        {
            isClicked = true;
            inputFieldAnimator.Play(inAnim);
            fieldTrigger.SetActive(true);
        }

        public void FieldTrigger()
        {
            if (isEmpty == true)
            {
                inputFieldAnimator.Play(outAnim);
                fieldTrigger.SetActive(false);
                isClicked = false;
            }

            else
            {
                fieldTrigger.SetActive(false);
                isClicked = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Animate();
            keyboardManager.SelectedInputFieldName(this.gameObject.name);
        }

        public void OnSelect(BaseEventData eventData)
        {            
            keyboardManager.SelectedInputFieldName(this.gameObject.name);            
        }
    }
}