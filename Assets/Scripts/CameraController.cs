using System;
using UnityEngine;

// Inspired by https://gist.github.com/JohannesMP/e15fe61386d4381d4441c3c324d96c56
public class CameraController : MonoBehaviour {
      
    float speed = 25.0f;
    float mouseSensitivity = 0.2f;

    private int minZoomLevel = 3;
    private int maxZoomLevel = 10;
    private int currentZoomLevel = 8;
    private float cameraPanFactor = 2f;

    private Vector3 oldMousePosition;
    private GameObject cameraContainer;
    private GameObject mainCamera;
    private GameObject cameraHitPoint;

    private void Awake()
    {
        cameraContainer = GameObject.Find("CameraContainer");
        mainCamera = GameObject.Find("Main Camera");
        cameraHitPoint = GameObject.Find("Camera Hit Point");
    }

    void FixedUpdate ()
    {
        // Set a marker to the point in space where the camera intersects with the world floor
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, 1 << Constants.worldFloorLayer))
        {
            Vector3 newPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            cameraHitPoint.transform.position = newPosition;
        }

        // Reset position on mouse btn down to prevent jumps
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            oldMousePosition = Input.mousePosition;
        }
        
        // Mouse movement
        if (Input.GetKey(KeyCode.Mouse2))
        {
            // Calculate delta
            Vector3 mouseDelta = Input.mousePosition - oldMousePosition;
            oldMousePosition = Input.mousePosition;

            // Multiply with sensitivity
            float yMovement = mouseDelta.x * mouseSensitivity;

            // Set to new rotation
            cameraContainer.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, yMovement);
        }

        // Keyboard movement of the camera container
        Vector3 cameraContainerMovement = new Vector3();

        // Zoom in with the scroll wheel
        if (Input.mouseScrollDelta.y > 0.0f && currentZoomLevel > minZoomLevel)
        {
            currentZoomLevel -= 1;
            cameraContainerMovement += new Vector3(0, -5, +10);

            // Make camera angle more shallow
            Vector3 mainCameraRotation = mainCamera.transform.rotation.eulerAngles;
            mainCameraRotation.x -= cameraPanFactor;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }

        // Zoom out with the scroll wheel
        if (Input.mouseScrollDelta.y < 0.0f && currentZoomLevel < maxZoomLevel)
        {
            currentZoomLevel += 1;
            cameraContainerMovement += new Vector3(0, +5, -10);

            // Make camera angle more steep
            Vector3 mainCameraRotation = mainCamera.transform.rotation.eulerAngles;
            mainCameraRotation.x += cameraPanFactor;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }
        
        // Fly over the board in all directions
        if (Input.GetKey(KeyCode.W)) cameraContainerMovement += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.A)) cameraContainerMovement += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) cameraContainerMovement += new Vector3(0, 0, -1);        
        if (Input.GetKey(KeyCode.D)) cameraContainerMovement += new Vector3(1, 0, 0);
        
        // Rotate Camera with Q & E
        if (Input.GetKey(KeyCode.E)) cameraContainer.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, 100f * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q)) cameraContainer.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, -100f * Time.deltaTime);

        // Add movement 
        cameraContainer.transform.Translate(cameraContainerMovement * speed * Time.deltaTime);
    }

    public void SetCamerarRotation(Quaternion cameraRotation)
    {
        mainCamera.transform.rotation = cameraRotation;
    }

    public void SetCameraRotation(Quaternion cameraRotation)
    {
        mainCamera.transform.rotation = cameraRotation;
    }

    public Quaternion GetCameraRotation()
    {
        return mainCamera.transform.rotation;
    }

    public void SetCameraContainerPosition(Vector3 cameraContainerPosition)
    {
        cameraContainer.transform.position = cameraContainerPosition;
    }

    public Vector3 GetCameraContainerPosition()
    {
        return cameraContainer.transform.position;
    }

    public void SetCameraContainerRotation(Quaternion cameraContainerRotation)
    {
        cameraContainer.transform.rotation = cameraContainerRotation;
    }

    public Quaternion GetCameraContainerRotation()
    {
        return cameraContainer.transform.rotation;
    }

    public void SetZoomLevel(int zoomLevel)
    {
        currentZoomLevel = zoomLevel;
    }

    public int GetZoomLevel()
    {
        return currentZoomLevel;
    }
}