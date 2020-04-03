using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    //private Material highlightMat;
    // Start is called before the first frame update
    
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        //highlightMat = Resources.Load<Material>("SteamVR/Resources/SteamVR_HoverHighlight");  
    }

    public void EnableHighlight(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
       
        //highlightMat.SetColor("yellow", Color.yellow);
        //gameObject.GetComponent<Renderer>().material = highlightMat;
            
        //gameObject.GetComponent<Renderer>().material = null;
    }
}
