﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARCanvasInteractionLog : MonoBehaviour
{
    //[SerializeField]
    //private TextMeshProUGUI details;
    
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnARObjectPlaced(ARPlacementInteractableSingle interactableSingle, GameObject placedObject)
    {

    }
}
