using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARCubeSpawner : MonoBehaviour
{
    [Header("References")]
    // Assign your cube prefab in the inspector
    public GameObject cubePrefab;
    // Reference to the AR Raycast Manager (usually attached to AR Session Origin)
    public ARRaycastManager raycastManager;
    // TMP_Text element to display the measured distance
    public TMP_Text distanceText;
    // Reference to the Line Renderer used to draw a line between cubes
    public LineRenderer lineRenderer;

    // List to keep track of spawned cubes (we only allow two at a time)
    private List<GameObject> cubes = new List<GameObject>();

    // A reusable list for storing raycast hit results
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Only process touches if there is at least one and we haven’t already spawned two cubes
        if (Input.touchCount > 0 && cubes.Count < 2)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                // Perform an AR raycast from the touch position against detected planes
                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    // Get the pose (position and rotation) of the hit
                    Pose hitPose = hits[0].pose;
                    // Spawn the cube at the hit position
                    SpawnCube(hitPose.position);
                }
            }
        }

        // If two cubes are present and a Line Renderer is assigned, update its positions.
        if (cubes.Count == 2 && lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, cubes[0].transform.position);
            lineRenderer.SetPosition(1, cubes[1].transform.position);
        }
    }

    // Spawns a cube at the given position and measures the distance if this is the second cube.
    void SpawnCube(Vector3 position)
    {
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
        cubes.Add(cube);

        // When two cubes are present, measure the distance between them
        if (cubes.Count == 2)
        {
            float distance = Vector3.Distance(cubes[0].transform.position, cubes[1].transform.position);
            if (distanceText != null)
            {
                distanceText.text = "Distance: " + distance.ToString("F2") + " m";
            }
        }
    }

    // Public method to reset the measurement (clear cubes, UI text, and line renderer).
    public void ResetMeasurement()
    {
        // Destroy all spawned cubes and clear the list
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();

        // Reset the UI text display
        if (distanceText != null)
        {
            distanceText.text = "Distance: 0 m";
        }

        // Clear the line renderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
