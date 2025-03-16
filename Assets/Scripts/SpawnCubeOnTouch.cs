using UnityEngine;

public class SpawnCubeOnTouch : MonoBehaviour
{
    [Tooltip("Prefab to spawn (a cube)")]
    public GameObject cubePrefab;

    [Tooltip("Reference to your AR Camera (should be a child of AR Session Origin)")]
    public Camera arCamera;

    [Tooltip("Distance from the camera at which the cube will be spawned")]
    public float spawnDistance = 2.0f;

    void Update()
    {
        // Check for a new touch on the screen.
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Get the touch position.
            Vector2 touchPosition = Input.GetTouch(0).position;
            
            // Cast a ray from the AR camera through the touch position.
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            
            // Calculate the spawn position at a fixed distance along the ray.
            Vector3 spawnPosition = ray.origin + ray.direction * spawnDistance;
            
            // Instantiate the cube at the calculated position.
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        }
    }
}