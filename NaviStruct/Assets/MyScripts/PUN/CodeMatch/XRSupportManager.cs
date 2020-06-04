using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSupportManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField]
    private Canvas startCanvas;
    [SerializeField]
    private Canvas keyboardCanvas;
    [SerializeField]
    private GameObject vrToggle;

    [Header("Camera Rig")]
    [SerializeField]
    private GameObject cameraRig;   
    [SerializeField]
    private XRController controller;
    
    [Header("Canvas Position Values")]
    public float startMenuDistance = 6f;
    public float keyboardDistance = 3f;
    public float speed = 1.5f;
    public float duration = 5f;  
    
    public bool isVRSupport;
    private bool isKeyboardActive;

    private AnimationController animController;

    private void OnEnable()
    {
        if (MasterManager.ClassReference.XRSupportManager == null)
            MasterManager.ClassReference.XRSupportManager = this;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        vrToggle.SetActive(true);
        if (XRDevice.isPresent)
        {                
            isVRSupport = true;
            vrToggle.GetComponent<Toggle>().isOn = true;            
        }
        else
        {
            isVRSupport = false;
            vrToggle.GetComponent<Toggle>().isOn = false;            
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        isVRSupport = false;
        vrToggle.SetActive(false);
        XRSettings.enabled = true;
#endif
    }

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;    
    }

    private void Update()
    {        
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool value) && isVRSupport)
        {
            if (value)
            {
                if(isKeyboardActive)
                    StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance));
                else
                    StartCoroutine(SetCanvasPosition(startCanvas,startMenuDistance));
            }
                
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
            StartCoroutine(animController.FadeKeyboard(animController.keyboardAnim, "isFade", true));
            isKeyboardActive = true;
            StartCoroutine(SetCanvasPosition(keyboardCanvas, keyboardDistance));
        }            
    }

    public void OnDeselectKeyboard()
    {
        if (isVRSupport)
        {                                 
            StartCoroutine(animController.FadeKeyboard(animController.keyboardAnim, "isFade", false));
            isKeyboardActive = false;
        }            
    }    

    private IEnumerator EnableVRSupport(bool activateDevice)
    {
        yield return new WaitForEndOfFrame();                        

        if (activateDevice)
        {
            if(XRSettings.enabled == false && XRSettings.loadedDeviceName != "OpenVR")
            {                
                XRSettings.LoadDeviceByName("OpenVR");
                yield return new WaitForEndOfFrame();
            }
            
            XRSettings.enabled = true;
            isVRSupport = true;

            yield return new WaitForEndOfFrame();           

            StartCoroutine(SetCanvasPosition(startCanvas, startMenuDistance));            
        }
        else
        {
            XRSettings.LoadDeviceByName("None");
            startCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            yield return new WaitForEndOfFrame();
            XRSettings.enabled = false;
            isVRSupport = false;
        }

        yield return null;
    }

    private IEnumerator SetCanvasPosition(Canvas canvas, float distance)
    {
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * distance;               

        float i = 0.0f;
        float rate = (1.0f / duration) * speed;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.transform.localScale = new Vector3(0.010f, 0.010f, 0.010f);

        while (i < 1)
        {
            Vector3 rotationOffset = Quaternion.LookRotation(canvas.transform.position - cameraRig.transform.position).eulerAngles;
            rotationOffset.x = 35;

            i += Time.deltaTime * rate;
            canvas.transform.position = Vector3.Lerp(canvas.transform.position, targetPos, i);

            if (isKeyboardActive)
                keyboardCanvas.transform.rotation = Quaternion.Euler(rotationOffset);
            else
                canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraRig.transform.position);               
                
            yield return new WaitForEndOfFrame();
        }        
    }
}
