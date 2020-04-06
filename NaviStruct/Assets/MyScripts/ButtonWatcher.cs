using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ButtonWatcher : MonoBehaviour
{
    public delegate void AxisChange(bool touched, Vector2 axis);
    public delegate void ButtonPressDelegate(bool pressed);

    public event ButtonPressDelegate primaryPressed;
    public event AxisChange axisTouch;

    private bool lastPrimaryButtonState = false;
    private bool lastAxisState = false;

    private Vector2 touchPos;   

    private List<InputDevice> allDevices;
    private List<InputDevice> devicesWithPrimaryButton;
    private List<InputDevice> devicesWith2DAxis;

    void Awake()
    {      
        allDevices = new List<InputDevice>();
        devicesWithPrimaryButton = new List<InputDevice>();
        devicesWith2DAxis = new List<InputDevice>();

        InputTracking.nodeAdded += InputTracking_nodeAdded;
    }

    // check for new input devices when new XRNode is added
    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        updateInputDevices();
    }

    // Update is called once per frame
    void Update()
    {
        OnAxisTouch();
        OnPrimaryPress();
    }


    public void ButtonState(InputDevice device)
    {
        bool tempButtonState = false;
        bool invalidDeviceFound = false;
    }        

    public void OnPrimaryPress()
    {
        bool tempStatePrimary = false;
        bool invalidPrimaryDeviceFound = false;

        foreach (InputDevice device in devicesWithPrimaryButton)
        {
            bool buttonState = false;
            tempStatePrimary = device.isValid && device.TryGetFeatureValue(CommonUsages.primaryButton, out buttonState) && buttonState || tempStatePrimary;

            if (!device.isValid)
                invalidPrimaryDeviceFound = true;
        }

        if (tempStatePrimary != lastPrimaryButtonState) // Button state changed since last frame
        {
            if(primaryPressed != null)
                primaryPressed(tempStatePrimary);

            lastPrimaryButtonState = tempStatePrimary;
        }

        if (invalidPrimaryDeviceFound || devicesWithPrimaryButton.Count == 0) // refresh device lists
            updateInputDevices();
    }

    public void OnAxisTouch()
    {
        
        bool tempStateAxisTouch = false;    
        bool tempStatePos = false;       
        bool invalidAxisDeviceFound = false;       

        foreach (InputDevice device in devicesWith2DAxis)
        {
            Vector2 tempPos;
            bool buttonStateTouch = false;            

            tempStateAxisTouch = device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out buttonStateTouch) && buttonStateTouch || tempStateAxisTouch;    
                        
            tempStatePos = device.TryGetFeatureValue(CommonUsages.primary2DAxis, out tempPos);
            touchPos = tempPos;

            if (!device.isValid)
                invalidAxisDeviceFound = true;
        }       

        if (tempStateAxisTouch != lastAxisState)
        {            
            axisTouch(tempStateAxisTouch, touchPos);            
            lastAxisState = tempStateAxisTouch;
        }       

        if (invalidAxisDeviceFound || devicesWith2DAxis.Count == 0)
            updateInputDevices();        
    }

    // find any devices supporting the desired feature usage
    void updateInputDevices()
    {
        devicesWithPrimaryButton.Clear();
        devicesWith2DAxis.Clear();

        InputDevices.GetDevices(allDevices);
        bool discardedValue;

        foreach (var device in allDevices)
        {
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out discardedValue))
            {
                devicesWithPrimaryButton.Add(device); // Add any devices that have a primary button.                
            }

            if (device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out discardedValue))
            {
                devicesWith2DAxis.Add(device); // Add any devices that have a primary button.                
            }
        }
    }

}
