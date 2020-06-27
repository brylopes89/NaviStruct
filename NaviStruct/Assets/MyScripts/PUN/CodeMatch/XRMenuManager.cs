using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using VRKeyboard.Utils;
using Photon.Realtime;
using System.Security.Cryptography;

public class XRMenuManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The button that will activate the menu")]
    InputHelpers.Button menuActivation;

    [SerializeField]
    [Tooltip("Gets or sets the the amount the axis needs to be pressed to trigger an interaction event.")]
    float axisToPressThreshold = 1f;

    [Header("Menu References")]
    [SerializeField]
    private Canvas startMenu;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private Canvas interactiveMenu;
    [SerializeField]
    private Canvas standaloneMenu;
    [SerializeField]
    private Toggle vrToggle;

    [Header("Camera Rig")]
    [SerializeField]
    private GameObject cameraRig;   
    [SerializeField]
    private XRController controller;

    private InputDevice m_RightController;    
    private InputDevice m_LeftController;

    [Header("Canvas Position Values")]
    public float menuDistance = 6f;
    public float keyboardDistance = 3f;
    public float speed = 1f;
    public float duration = 2f;  
    
    public bool isVRSupport;

    private bool isKeyboardActive = false;   
    private bool isToggleMenu = false;   

    private AnimationController animController;
    private Scene scene;   

    private void OnEnable()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;      

        scene = SceneManager.GetActiveScene();    
        
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (scene.buildIndex == 0)
        {            
            if (XRDevice.isPresent)                          
                vrToggle.isOn = true;           
            else
            {
                vrToggle.isOn = false;
                vrToggle.onValueChanged.Invoke(vrToggle.isOn);
            }                                      
        }

        isVRSupport = MasterManager.ClassReference.IsVRSupport;

        if (scene.buildIndex == 1)
        {            
            if (isVRSupport)                            
                standaloneMenu.gameObject.SetActive(false);
            else
                standaloneMenu.gameObject.SetActive(true);
        }

#elif UNITY_ANDROID || UNITY_IOS
        if (scene.buildIndex == 0)
        {
            temp = false;
            vrToggle.SetActive(false);
            XRSettings.enabled = true;
        }
#endif        
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

            yield return new WaitForEndOfFrame();
            StartCoroutine(SetCanvasPosition(startMenu, menuDistance, 5));
        }
        else
        {
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
