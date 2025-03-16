using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class MeasurementRaycastSpawner : MonoBehaviour
{

    [Header("References")]
    // Assign your cube prefab in the inspector
    public GameObject cubePrefab;
    // Reference to the AR Raycast Manager (usually attached to AR Session Origin)
    public ARRaycastManager raycastManager;
    // TMP_Text element to display the measured distance
    public TMP_Text distanceText;

    // List to keep track of spawned cubes (we only allow two at a time)
    private List<GameObject> cubes = new List<GameObject>();

    // A reusable list for storing raycast hit results
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Reference to the instantiated Line Renderer (instantiated once both cubes are in the scene)
    private LineRenderer lineRenderer;

    void Update()
    {
        // Process screen touches if we haven't already spawned two cubes.
        if (Input.touchCount > 0 && cubes.Count < 2)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                // Perform an AR raycast from the touch position against detected planes.
                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    SpawnCube(hitPose.position);
                }
            }
        }

        // When two cubes are present, instantiate the Line Renderer (if not already instantiated) and update its positions.
        if (cubes.Count == 2)
        {
            if (lineRenderer == null)
            {
                // Create a new GameObject to hold the Line Renderer component.
                GameObject lineRendererObj = new GameObject("LineRendererObject");
                lineRenderer = lineRendererObj.AddComponent<LineRenderer>();
                // Optionally configure the Line Renderer.
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                // Use a basic material. You may assign a custom material if desired.
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, cubes[0].transform.position);
            lineRenderer.SetPosition(1, cubes[1].transform.position);
        }
    }

    // Spawns a cube at the specified position. If two cubes exist, measures the distance between them.
    void SpawnCube(Vector3 position)
    {
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
        cubes.Add(cube);

        // When two cubes are present, calculate the distance and update the TMP text.
        if (cubes.Count == 2)
        {
            float distance = Vector3.Distance(cubes[0].transform.position, cubes[1].transform.position);
            if (distanceText != null)
            {
                distanceText.text = "Distance: " + distance.ToString("F2") + " m";
            }
        }
    }

    // Public method to reset the measurement by destroying the cubes, resetting the text, and destroying the line renderer.
    public void ResetMeasurement()
    {
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();

        if (distanceText != null)
        {
            distanceText.text = "Distance: 0 m";
        }

        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
            lineRenderer = null;
        }
    }
}

