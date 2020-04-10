using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro debugAreaText;

    [SerializeField]
    private bool enableDebug = false;

    [SerializeField]
    private int maxLines = 15;

    // Start is called before the first frame update
    void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;
    }

    // Update is called once per frame
    public void LogInfo(string message)
    {
        
    }
}
