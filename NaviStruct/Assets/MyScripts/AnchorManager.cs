using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine.EventSystems;

public class AnchorManager : MonoBehaviour
{
    public Camera FirstPersonCamera;
    public GameObject anchoredPrefab;
    public GameObject ManipulatorPrefab;

    private Anchor anchor;
    private List<GameObject> prefabs = new List<GameObject>();

    private float k_PrefabRotation = 180;
    private bool isSpawned = false;    

    // Update is called once per frame
    void Update()
    {   
        Touch touch;

        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            if (!isSpawned && prefabs.Count < 1)
            {
                isSpawned = true;
                SpawnGameObject(anchoredPrefab, hit.Pose.position, hit.Pose.rotation, hit.Trackable.CreateAnchor(hit.Pose));
            }
        }

        else
            isSpawned = false;
    }

    private void SpawnGameObject(GameObject go, Vector3 pos, Quaternion rot, Anchor createAnchor)
    {
        // Instantiate prefab at the hit pose.
        var gameObject = Instantiate(go, pos, rot);

        var manipulator = Instantiate(ManipulatorPrefab, pos, rot);

        // Compensate for the hitPose rotation facing away from the raycast (i.e.
        // camera).
        gameObject.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

        // Create an anchor to allow ARCore to track the hitpoint as understanding of
        // the physical world evolves.
        anchor = createAnchor;

        // Make game object a child of the anchor.
        gameObject.transform.parent = anchor.transform;

        // Make manipulator a child of the anchor.
        manipulator.transform.parent = anchor.transform;

        // Select the placed object.
        manipulator.GetComponent<Manipulator>().Select();

        prefabs.Add(gameObject);
    }
}
