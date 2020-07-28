using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARSessionManager : MonoBehaviour
{
    [SerializeField]
    private ARPlacementInteractableSingle interactableSingle;

    [SerializeField]
    private ARPlaneManager planeManager;

    [SerializeField]
    private Button togglePlaneButton;

    bool isActive;

    public void ClearAllObjects()
    {
        isActive = !isActive;

       interactableSingle.TogglePlacementObject();
    }

    public void TogglePlanes()
    {
        planeManager.enabled = !planeManager.enabled;
        ChangeStateOfPlanes(planeManager.enabled);

        var textForToggle = togglePlaneButton.GetComponentInChildren<TextMeshProUGUI>();
        textForToggle.text = planeManager.enabled ? "DISABLE PLANE" : "ENABLE PLANE";
    }   

    private void ChangeStateOfPlanes(bool state)
    {
        var planes = planeManager.trackables;

        foreach (ARPlane arPlane in planes)
            arPlane.gameObject.SetActive(state);
    }
}
