using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRMenuInput : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The button that will activate the menu")]
    InputHelpers.Button MenuActivation;

    [SerializeField]
    [Tooltip("The button that will select from menu options")]
    InputHelpers.Button Select;

    [SerializeField]
    [Tooltip("Gets or sets the the amount the axis needs to be pressed to trigger an interaction event.")]
    float m_AxisToPressThreshold = 0.1f;  

    public RadialMenu menu = null;
    public bool isPress = false;

    InputDevice m_RightController;
    InputDevice m_LeftController;    

    // Start is called before the first frame update
    void OnEnable()
    {
        InputDevices.deviceConnected += RegisterDevices;      
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
            
            m_LeftController.IsPressed(MenuActivation, out bool value);
            activated |= value;            

            

            if (activated)
            {
                isPress = !isPress;
                menu.Show(isPress);
            }

                                 
        }

        if (m_RightController.isValid)
        {               
                              
        }
    }
}
