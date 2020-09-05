using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using VRKeyboard.Utils;
using TMPro;
using Photon.Pun;

public class XRMenuManager : MonoBehaviour
{
    [Header("Menu References")] 
    [SerializeField]
    private Canvas startMenuCanvas;
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
    [SerializeField]
    private GameObject tipText;      
    [SerializeField]
    private XRController controller;

    [Header("Canvas Position Values")]
    public float menuDistance = 6f;   
    public float offset;
    public float speed = 1f;
    public float duration = 2f;   
    public bool isVRSupport;
    public bool isARSupport;

    private bool isKeyboardActive = false;   
    private bool isToggleMenu = false;    

    private AnimationController animController;
    private Scene scene;

    private Vector3 world_Canvas_Scale;    
    private Canvas target_Canvas;

    private GameObject avatar_Instance;

    private void Awake()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;      

        scene = SceneManager.GetActiveScene();       
        world_Canvas_Scale = new Vector3(0.01f, 0.01f, 0.01f);        
    }

    private void OnEnable()
    {        
        animController = MasterManager.ClassReference.AnimController;        

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
            startMenuCanvas.transform.localScale = world_Canvas_Scale;
            statusText.transform.localScale = new Vector3(4f, 4f, 4f);
            tipText.transform.localScale = Vector3.one;

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                startMenuChildObjects[i].transform.localScale = Vector3.one;

            if (XRDevice.isPresent)
                vrToggle.isOn = true;
            else
                vrToggle.isOn = false;
#endif
        }
        else
        {
            avatar_Instance = MasterManager.ClassReference.Avatar;
        }       
    }

    public void OpenInteractiveMenuOnClick()
    {
        isToggleMenu = !isToggleMenu;
        menuDistance = 3f;
        offset = 9f;
        StartCoroutine(animController.FadeCanvas(animController.interactiveMenu, animController.interactiveMenuAnim, "IsFadeOut", isToggleMenu));
        StartCoroutine(interactiveMenu.GetComponent<UIMenu>().SetCanvasPosition(menuDistance, offset));
    }

    public void SetCanvasPosition()
    {        
        if (scene.buildIndex == 0)
        {
            if (isKeyboardActive)
            {
                target_Canvas = keyboardCanvas;
                offset = 20f;
                menuDistance = 3f;               
            }
            else
            {
                target_Canvas = startMenuCanvas;
                offset = 5f;
                menuDistance = 6f;               
            }

            StartCoroutine(target_Canvas.GetComponent<UIMenu>().SetCanvasPosition(menuDistance, offset));
        }
        else
        {
            target_Canvas = interactiveMenu;     
            
            if(isToggleMenu)
                StartCoroutine(target_Canvas.GetComponent<UIMenu>().SetCanvasPosition(menuDistance, offset));
        }        
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
            menuDistance = 3f;
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", true));
            StartCoroutine(keyboardCanvas.GetComponent<UIMenu>().SetCanvasPosition(menuDistance, 20));
        }            
    }

    public void OnDeselectKeyboard()
    {
        if (isVRSupport)
        {
            isKeyboardActive = false;
            menuDistance = 6f;
            StartCoroutine(animController.FadeCanvas(animController.keyboard, animController.keyboardAnim, "isFade", false));              
            keyboardCanvas.GetComponent<KeyboardManager>().ClearOnDeselect();           
        }            
    } 

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();
        
        isVRSupport = activateDevice;                

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

            startMenuCanvas.renderMode = RenderMode.WorldSpace;
            startMenuCanvas.worldCamera = Camera.main;
            startMenuCanvas.transform.localScale = world_Canvas_Scale;

            yield return new WaitForEndOfFrame();
            
            StartCoroutine(startMenuCanvas.GetComponent<UIMenu>().SetCanvasPosition(menuDistance, 5));
        }
        else
        {
            controller.GetComponent<LineRenderer>().enabled = false;
            controller.GetComponent<XRInteractorLineVisual>().enabled = false;

            yield return new WaitForSeconds(1f);
            XRSettings.LoadDeviceByName("None");
            startMenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;            
        }

        yield return null;
    }
}
