using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRControllerManager : MonoBehaviour
{
    [SerializeField]    
    List<InputHelpers.Button> m_ActivationButtons = new List<InputHelpers.Button>();
    public List<InputHelpers.Button> activationButtons { get { return m_ActivationButtons; } set { m_ActivationButtons = value; } }

    [SerializeField]    
    List<InputHelpers.Button> m_DeactivationButtons = new List<InputHelpers.Button>();

    public List<InputHelpers.Button> deactivationButtons { get { return m_DeactivationButtons; } set { m_DeactivationButtons = value; } }    

    InputDevice m_RightController;
    InputDevice m_LeftController;

    public XRRaycast lineGrab;

    // Start is called before the first frame update
    void OnEnable()
    {
        InputDevices.deviceConnected += RegisterDevices;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        for (int i = 0; i < devices.Count; i++)
            RegisterDevices(devices[i]);
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= RegisterDevices;
    }

    void RegisterDevices(InputDevice connectedDevice)
    {
        if (connectedDevice.isValid)
        {
#if UNITY_2019_3_OR_NEWER
            if ((connectedDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
#else
            if (connectedDevice.role == InputDeviceRole.LeftHanded)
#endif
            {
                m_LeftController = connectedDevice;
            }
#if UNITY_2019_3_OR_NEWER
            else if ((connectedDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
#else
            else if (connectedDevice.role == InputDeviceRole.RightHanded)
#endif
            {
                m_RightController = connectedDevice;
            }
        }
    }

    void Update()
    {
        if (m_LeftController.isValid)
        {
            bool activated = false;
            for (int i = 0; i < m_ActivationButtons.Count; i++)
            {
                m_LeftController.IsPressed(m_ActivationButtons[i], out bool value);
                activated |= value;
            }            

            bool deactivated = false;
            for (int i = 0; i < m_DeactivationButtons.Count; i++)
            {
                m_LeftController.IsPressed(m_DeactivationButtons[i], out bool value);
                deactivated |= value;
            }            
        }

        if (m_RightController.isValid)
        {
            bool activated = false;
            for (int i = 0; i < m_ActivationButtons.Count; i++)
            {
                m_RightController.IsPressed(m_ActivationButtons[i], out bool value);
                activated |= value;
            }

            if (activated)
            {
               // lineGrab.setMove = true;                
            }              
                         

            bool deactivated = false;
            for (int i = 0; i < m_DeactivationButtons.Count; i++)
            {
                m_RightController.IsPressed(m_DeactivationButtons[i], out bool value);
                deactivated |= value;
            }

            if (deactivated)
            {
               // lineGrab.setMove = false;
                
            }
                
                              
        }
    }
}
