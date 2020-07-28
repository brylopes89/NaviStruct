using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementInteractableSingle : ARBaseGestureInteractable
{
    [SerializeField, Tooltip("A GameObject to place when a raycast from a user touch hits a plane.")]    
    private GameObject m_PlacementPrefab;
    /// <summary>
    /// A GameObject to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject placementPrefab { get { return m_PlacementPrefab; } set { m_PlacementPrefab = value; } }

    [SerializeField, Tooltip("Called when this interactable places a new GameObject in the world.")]
    ARObjectPlacementEvent m_OnObjectPlaced = new ARObjectPlacementEvent();
    /// <summary>Gets or sets the event that is called when the this interactable places a new GameObject in the world.</summary>
    public ARObjectPlacementEvent onObjectPlaced { get { return m_OnObjectPlaced; } set { m_OnObjectPlaced = value; } }

    [HideInInspector]
    public GameObject placementObject;    

    private static GameObject trackablesObject;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();   
    private List<GameObject> prefab = new List<GameObject>();
    private ChangeModeController modeController;

    private bool isActive;

    private void Start()
    {
        modeController = FindObjectOfType<ChangeModeController>();
    }

    /// <summary>
    /// Returns true if the manipulation can be started for the given gesture.
    /// </summary>
    /// <param name="gesture">The current gesture.</param>
    /// <returns>True if the manipulation can be started.</returns>
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null)
            return true;

        return false;
    }   

    /// <summary>
    /// Function called when the manipulation is ended.
    /// </summary>
    /// <param name="gesture">The current gesture.</param>
    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled)
            return;

        // If gesture is targeting an existing object we are done.
        // Allow for test planes
        if (gesture.TargetObject != null)
            return;

        // Raycast against the location the player touched to search for planes.
        if (GestureTransformationUtility.Raycast(gesture.StartPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hit = hits[0];

            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if (Vector3.Dot(Camera.main.transform.position - hit.pose.position, hit.pose.rotation * Vector3.up) < 0)            
                return;            

            if(prefab.Count < 1)
            {
                // Instantiate placement prefab at the hit pose.
                placementObject = Instantiate(placementPrefab, hit.pose.position, hit.pose.rotation);

                // Create anchor to track reference point and set it as the parent of placementObject.
                var anchorObject = new GameObject("PlacementAnchor");
                anchorObject.transform.position = hit.pose.position;
                anchorObject.transform.rotation = hit.pose.rotation;
                placementObject.transform.parent = anchorObject.transform;

                // Find trackables object in scene and use that as parent
                if (trackablesObject == null)                
                    trackablesObject = GameObject.Find("Trackables");                

                if (trackablesObject != null)
                    anchorObject.transform.parent = trackablesObject.transform;

                m_OnObjectPlaced?.Invoke(this, placementObject);

                modeController.AssignARPlayground(placementObject);
                prefab.Add(placementObject);
            }
        }
    }

    public void TogglePlacementObject()
    {
        isActive = !isActive;
        placementObject.SetActive(isActive);
    }
}
