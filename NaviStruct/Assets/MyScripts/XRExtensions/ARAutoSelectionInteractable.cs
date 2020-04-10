using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.Interaction.Toolkit;

public class ARAutoSelectionInteractable : ARSelectionInteractable
{
    public bool m_GestureSelected { get; private set; } = true;

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        return true;
    }
}
