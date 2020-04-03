using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction;

[System.Serializable]
public class PrimaryButtonEvent : UnityEvent<bool> { }

public class ButtonWatcher : MonoBehaviour
{
    public delegate void axisChange(bool touched, Vector2 axis);

    public PrimaryButtonEvent primaryButtonPress;    

    public event axisChange axisTouch;

    private bool lastPrimaryButtonState = false;
    private bool lastAxisState = false;
    Vector2 touchPos;   

    private List<InputDevice> allDevices;
    private List<InputDevice> devicesWithPrimaryButton;
    private List<InputDevice> devicesWith2DAxis;

    void Awake()
    {        
        if (primaryButtonPress == null)        
            primaryButtonPress = new PrimaryButtonEvent();        

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
    }

    public void OnAxisTouch()
    {
        bool tempStatePrimary = false;
        bool tempStateAxis = false;
        bool tempStatePos = false;

        bool invalidPrimaryDeviceFound = false;
        bool invalidAxisDeviceFound = false;

        foreach (InputDevice device in devicesWithPrimaryButton)
        {
            bool buttonState = false;
            tempStatePrimary = device.isValid && device.TryGetFeatureValue(CommonUsages.primaryButton, out buttonState) && buttonState || tempStatePrimary;       

            if (!device.isValid)
                invalidPrimaryDeviceFound = true;
        }

        foreach (InputDevice device in devicesWith2DAxis)
        {
            Vector2 tempPos;
            bool buttonState = false;

            tempStateAxis = device.isValid && device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out buttonState) && buttonState || tempStateAxis;
            tempStatePos = device.TryGetFeatureValue(CommonUsages.primary2DAxis, out tempPos);
            touchPos = tempPos;

            if (!device.isValid)
                invalidAxisDeviceFound = true;
        }

        if (tempStatePrimary != lastPrimaryButtonState) // Button state changed since last frame
        {
            primaryButtonPress.Invoke(tempStatePrimary);
            
            lastPrimaryButtonState = tempStatePrimary;
        }

        if (tempStateAxis != lastAxisState)
        {            
            axisTouch.Invoke(tempStateAxis, touchPos);
            lastAxisState = tempStateAxis;
        }

        if (invalidPrimaryDeviceFound || devicesWithPrimaryButton.Count == 0) // refresh device lists
            updateInputDevices();

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
