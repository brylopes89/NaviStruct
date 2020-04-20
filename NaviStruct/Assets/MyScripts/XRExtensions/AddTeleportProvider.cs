using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AddTeleportProvider : MonoBehaviour
{    
    TeleportationArea teleportArea;

    private void Awake()
    {
        teleportArea = FindObjectOfType<TeleportationArea>();
    }

    private void Start()
    {
        teleportArea.teleportationProvider = this.GetComponent<TeleportationProvider>();
    }
}
