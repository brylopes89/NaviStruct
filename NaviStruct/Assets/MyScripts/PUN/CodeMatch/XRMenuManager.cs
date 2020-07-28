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
    public float keyboardDistance = 3f;
    public float speed = 1f;
    public float duration = 2f;     
    
    public bool isVRSupport;
    public bool isARSupport;

    public Vector3 statusOffset;

    private float scaleMultiplier = 1.4f;

    private bool isKeyboardActive = false;   
    private bool isToggleMenu = false;

    private Vector3 originalStartMenuScale;
    private Vector3 originalmenuChildrenScale;
    private Vector3 originalLobbyTextScale;

    private AnimationController animController;
    private Scene scene;   

    private void Awake()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;      

        scene = SceneManager.GetActiveScene();       

        if (scene.buildIndex == 0)
        {
            originalStartMenuScale = new Vector3(0.01f, 0.01f, 0.01f);
            originalLobbyTextScale = Vector3.one;

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                originalmenuChildrenScale = Vector3.one;
      

#if UNITY_EDITOR && UNITY_ANDROID || UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID || UNITY_IOS

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                startMenuChildObjects[i].transform.localScale = startMenuChildObjects[i].transform.localScale * scaleMultiplier;

            isVRSupport = false;
            isARSupport = true;
            MasterManager.ClassReference.IsARSupport = isARSupport;

            startMenu.renderMode = RenderMode.ScreenSpaceOverlay;       
            vrToggle.gameObject.SetActive(false);
            xrSupportText.enabled = false;            

            lobbyText.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);            
            statusText.transform.localScale = new Vector3(2f, 2f, 2f);

            statusOffset.y = -100f;
            statusText.transform.position = statusText.transform.position + statusOffset;

#else
            xrSupportText.enabled = true;
            isARSupport = false;

            startMenu.transform.localScale = originalStartMenuScale;
            lobbyText.transform.localScale = originalLobbyTextScale;
            statusText.transform.localScale = new Vector3(4f, 4f, 4f);

            statusOffset.y = -2f;
            statusText.transform.position = statusText.transform.position + statusOffset;

            for (int i = 0; i < startMenuChildObjects.Length; i++)
                startMenuChildObjects[i].transform.localScale = originalmenuChildrenScale;

            if (XRDevice.isPresent)
            {          
                vrToggle.isOn = true;               
            }                           
            else
            {
                vrToggle.isOn = false;
                XRSettings.LoadDeviceByName("None");
                startMenu.renderMode = RenderMode.ScreenSpaceOverlay;
                XRSettings.enabled = false;
            }   
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
                    StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance, 20));
                else
                    StartCoroutine(SetCanvasPosition(startMenu, menuDistance, 5));
            }
            else
            {
                OpenInteractiveMenuOnClick(activate);
            }
        }

        if (isToggleMenu)
            StartCoroutine(SetCanvasPosition(interactiveMenu, keyboardDistance, 8));
    }

    public void OpenInteractiveMenuOnClick(bool isToggle)
    {
        isToggleMenu = !isToggleMenu;
        StartCoroutine(animController.FadeCanvas(animController.interactiveMenu, animController.interactiveMenuAnim, "IsFadeOut", isToggleMenu));
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
            StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance, 20));
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
            cameraRig.GetComponentInChildren<LineRenderer>().enabled = true;

            yield return new WaitForEndOfFrame();
            startMenu.transform.localScale = originalStartMenuScale;
            StartCoroutine(SetCanvasPosition(startMenu, menuDistance, 5));
        }
        else
        {
            foreach(LineRenderer renderer in cameraRig.GetComponentsInChildren<LineRenderer>())
            {
                renderer.enabled = false;
            }            
            yield return new WaitForSeconds(.7f);

            XRSettings.LoadDeviceByName("None");
            startMenu.renderMode = RenderMode.ScreenSpaceOverlay;

            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;            
        }

        yield return null;
    }

    private IEnumerator SetCanvasPosition(Canvas canvas, float distance, float offset)
    {
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * distance;               

        float i = 0.0f;
        float rate = (1.0f / duration) * speed;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;        

        while (i < 1)
        {
            Vector3 rotationOffset = Quaternion.LookRotation(canvas.transform.position - cameraRig.transform.position).eulerAngles;
            rotationOffset.x = offset;
            i += Time.deltaTime * rate;

            canvas.transform.position = Vector3.Lerp(canvas.transform.position, targetPos, i);             
            canvas.transform.rotation = Quaternion.Euler(rotationOffset);                                           
                
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
