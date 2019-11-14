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

    private GameObject cameraContainerTarget;
    private GameObject mainCameraTarget;

    private void Awake()
    {
        cameraHitPoint = GameObject.Find("Camera Hit Point");

        cameraContainer = GameObject.Find("CameraContainer");
        mainCamera = GameObject.Find("Main Camera");

        cameraContainerTarget = GameObject.Find("Camera Container Target");
        mainCameraTarget = GameObject.Find("Main Camera Target");
    }

    void Update ()
    {
        // Set a marker to the point in space where the camera intersects with the world floor
        RaycastHit hit;
        if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Mouse2) && Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, 1 << Constants.worldFloorLayer))
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
            cameraContainerTarget.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, yMovement);
        }

        // Keyboard movement of the camera container
        Vector3 cameraContainerMovement = new Vector3();

        // Zoom in with the scroll wheel
        if (Input.mouseScrollDelta.y > 0.0f && currentZoomLevel > minZoomLevel)
        {
            currentZoomLevel -= 1;
            cameraContainerMovement += new Vector3(0, -5, +10);

            // Make camera angle more shallow
            Vector3 mainCameraRotation = mainCameraTarget.transform.rotation.eulerAngles;
            mainCameraRotation.x -= cameraPanFactor;
            mainCameraTarget.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }

        // Zoom out with the scroll wheel
        if (Input.mouseScrollDelta.y < 0.0f && currentZoomLevel < maxZoomLevel)
        {
            currentZoomLevel += 1;
            cameraContainerMovement += new Vector3(0, +5, -10);

            // Make camera angle more steep
            Vector3 mainCameraRotation = mainCameraTarget.transform.rotation.eulerAngles;
            mainCameraRotation.x += cameraPanFactor;
            mainCameraTarget.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }
        
        // Fly over the board in all directions
        if (Input.GetKey(KeyCode.W)) cameraContainerMovement += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.A)) cameraContainerMovement += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) cameraContainerMovement += new Vector3(0, 0, -1);        
        if (Input.GetKey(KeyCode.D)) cameraContainerMovement += new Vector3(1, 0, 0);
        
        // Rotate Camera with Q & E
        if (Input.GetKey(KeyCode.E)) cameraContainerTarget.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, 100f * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q)) cameraContainerTarget.transform.RotateAround(cameraHitPoint.transform.position, Vector3.up, -100f * Time.deltaTime);

        // Add movement 
        cameraContainerTarget.transform.Translate(cameraContainerMovement * speed * Time.deltaTime);

        // Move actual camera
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, mainCameraTarget.transform.rotation, Time.deltaTime * 5f);
        cameraContainer.transform.rotation = Quaternion.Lerp(cameraContainer.transform.rotation,cameraContainerTarget.transform.rotation , Time.deltaTime * 4f);
        cameraContainer.transform.position = Vector3.Lerp(cameraContainer.transform.position, cameraContainerTarget.transform.position, Time.deltaTime * 5f);
    }

    public void SetCameraRotation(Quaternion cameraRotation)    
    {
        mainCameraTarget.transform.rotation = cameraRotation;
    }

    public Quaternion GetCameraRotation()
    {
        return mainCameraTarget.transform.rotation;
    }

    public void SetCameraContainerPosition(Vector3 cameraContainerPosition)
    {
        cameraContainerTarget.transform.position = cameraContainerPosition;
    }

    public Vector3 GetCameraContainerPosition()
    {
        return cameraContainerTarget.transform.position;
    }

    public void SetCameraContainerRotation(Quaternion cameraContainerRotation)
    {
        cameraContainerTarget.transform.rotation = cameraContainerRotation;
    }

    public Quaternion GetCameraContainerRotation()
    {
        return cameraContainerTarget.transform.rotation;
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