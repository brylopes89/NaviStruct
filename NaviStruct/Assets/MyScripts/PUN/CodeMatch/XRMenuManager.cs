using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using VRKeyboard.Utils;
using TMPro;

public class XRMenuManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The button that will activate the menu")]
    InputHelpers.Button menuActivation;

    [Header("Menu References")] 
    [SerializeField]
    private Canvas startMenu;
    [SerializeField]
    private GameObject[] startMenuChildObjects;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private Canvas interactiveMenu;
    [SerializeField]
    private Toggle vrToggle;
    [SerializeField]
    private TextMeshProUGUI xrSupportText;
    [SerializeField]
    private TextMeshProUGUI lobbyText;
    [SerializeField]
    private GameObject statusText;

    [Header("Camera Rig")]
    [SerializeField]
    private GameObject cameraRig;   
    [SerializeField]
    private XRController controller;

    [Header("Canvas Position Values")]
    public float menuDistance = 6f;
    public float keyboardDistance = 5f;
    public float speed = 1f;
    public float duration = 2f;     
    
    public bool isVRSupport;
    public bool isARSupport;

    private bool isKeyboardActive = false;   
    private bool isToggleMenu = false;

    private AnimationController animController;
    private Scene scene;    

    private void Awake()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;      

        scene = SceneManager.GetActiveScene();       

        if (scene.buildIndex == 0)
        {

#if UNITY_EDITOR && UNITY_ANDROID || UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID || UNITY_IOS
            
            isVRSupport = false;
            isARSupport = true;

            Vector3 statusOffset = new Vector3(0, -100, 0);  
            float scaleMultiplier = 1.4f;

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                startMenuChildObjects[i].transform.localScale = startMenuChildObjects[i].transform.localScale * scaleMultiplier;   
                
            lobbyText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);            
            statusText.transform.localScale = new Vector3(2f, 2f, 2f);            
            statusText.transform.position = statusText.transform.position + statusOffset;

            startMenu.renderMode = RenderMode.ScreenSpaceOverlay;       
            vrToggle.gameObject.SetActive(false);

#else
            xrSupportText.enabled = true;
            isARSupport = false;

            lobbyText.transform.localScale = Vector3.one;
            startMenu.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);            
            statusText.transform.localScale = new Vector3(4f, 4f, 4f);      

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                startMenuChildObjects[i].transform.localScale = Vector3.one;

            if (XRDevice.isPresent)                     
                vrToggle.isOn = true;                                           
            else            
                vrToggle.isOn = false;                  
#endif
        }

        isVRSupport = MasterManager.ClassReference.IsVRSupport;
        isARSupport = MasterManager.ClassReference.IsARSupport;
    }

    private void Start()
    {        
        animController = MasterManager.ClassReference.AnimController;        
    }

    private void Update()
    {
        ActivateCanvas();        
    }

    private void ActivateCanvas()
    {
        bool activate = false;
        controller.inputDevice.IsPressed(menuActivation, out bool value);
        activate |= value;

        if (activate)
        {
            if (scene.buildIndex == 0)
            {
                if (isKeyboardActive)
                {                    
                    keyboardCanvas.GetComponent<UIMenu>().SetInteractiveMenuPosition(keyboardDistance, 20);
                }
                else
                {
                    startMenu.GetComponent<UIMenu>().SetInteractiveMenuPosition(menuDistance, 5);
                }                    
            }
            else
            {
                OpenInteractiveMenuOnClick(activate);
            }           
        }      
    }

    public void OpenInteractiveMenuOnClick(bool isToggle)
    {
        isToggleMenu = !isToggleMenu;
        StartCoroutine(animController.FadeCanvas(animController.interactiveMenu, animController.interactiveMenuAnim, "IsFadeOut", isToggleMenu));
        interactiveMenu.GetComponent<UIMenu>().SetInteractiveMenuPosition(menuDistance, 9f);
    }

    public void VRToggleOnClick(bool isToggle)
    {        
        StartCoroutine(EnableVRSupport(isToggle));
    }   

    public void OnSelectKeyboard()
    {
        if (isVRSupport)
        {
            isKeyboardActive = true;            
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", true));                 
        }            
    }

    public void OnDeselectKeyboard()
    {
        if (isVRSupport)
        {
            isKeyboardActive = false;           
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", false));            
            keyboardCanvas.gameObject.GetComponent<KeyboardManager>().Clear();           
        }            
    } 

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();        
        MasterManager.ClassReference.IsVRSupport = activateDevice;
        isVRSupport = MasterManager.ClassReference.IsVRSupport;

        if (activateDevice)
        {            
            if(XRSettings.enabled == false && XRSettings.loadedDeviceName != "OpenVR")
            {                
                XRSettings.LoadDeviceByName("OpenVR");
                yield return new WaitForEndOfFrame();
            }
            
            XRSettings.enabled = true;
            controller.GetComponent<LineRenderer>().enabled = true;
            controller.GetComponent<XRInteractorLineVisual>().enabled = true;

            yield return new WaitForEndOfFrame();            
            keyboardCanvas.GetComponent<UIMenu>().SetInteractiveMenuPosition(keyboardDistance, 20);
            startMenu.GetComponent<UIMenu>().SetInteractiveMenuPosition(menuDistance, 5);
        }
        else
        {
            controller.GetComponent<LineRenderer>().enabled = false;
            controller.GetComponent<XRInteractorLineVisual>().enabled = false;

            yield return new WaitForSeconds(.4f);
            XRSettings.LoadDeviceByName("None");
            startMenu.renderMode = RenderMode.ScreenSpaceOverlay;

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;            
        }

        yield return null;
    }
}
