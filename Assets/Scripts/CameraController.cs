using System;
using UnityEngine;

// Inspired by https://gist.github.com/JohannesMP/e15fe61386d4381d4441c3c324d96c56
public class CameraController : MonoBehaviour {
      
    public float speed = 25.0f;
    public float mouseSensitivity = 0.2f;

    public float cameraRotationLerpSpeed = 3f;
    public float cameraPositionLerpSpeed = 3f;

    public int minZoomSteps = 3;
    public int maxZoomSteps = 10;
    public int currentStep = 8;

    public float zoomTiltStep = 2f;
    public float zoomOffsetYStep = 5f;
    public float zoomOffsetZStep = -10f;

    private Vector3 oldMousePosition;

    public GameObject cameraCenter;
    public GameObject cameraOffset;
    public GameObject mainCamera;

    public GameObject cameraCenterTarget;
    public GameObject cameraOffsetTarget;
    public GameObject mainCameraTarget;

    public GameObject cameraHitPoint;

    private Vector3 currentPosition;
    private Vector3 targetPosition;

    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private Vector3 targetOffset;
    private Vector3 currentOffset;

    private Quaternion currentTilt;
    private Quaternion targetTilt;

    private void Awake()
    {
        currentRotation = cameraCenter.transform.rotation;
        targetRotation = cameraCenter.transform.rotation;

        currentPosition = cameraCenter.transform.position;
        targetPosition = cameraCenter.transform.position;
    }

    void Update ()
    {
        // Set a marker to the point in space where the camera intersects with the world floor
        RaycastHit hit;
        Vector3 startPosition = cameraCenterTarget.transform.position;
        startPosition.y = 100;

        // Test the height of the center.
        if (Physics.Raycast(
                startPosition,
                Vector3.down,
                out hit,
                1000,
                LayerMask.GetMask(
                    LayerMask.LayerToName(Constants.worldFloorLayer),
                    LayerMask.LayerToName(Constants.tileLayer))))
        {
            cameraHitPoint.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            targetPosition.y = hit.point.y;
        }

        // Reset position on mouse btn down to prevent jumps
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            oldMousePosition = Input.mousePosition;
        }
        
        // Calculate camera rotation with middle mouse button
        if (Input.GetKey(KeyCode.Mouse2))
        {
            // Calculate delta
            Vector3 mouseDelta = Input.mousePosition - oldMousePosition;
            oldMousePosition = Input.mousePosition;

            // Multiply with sensitivity
            float yMovement = mouseDelta.x * mouseSensitivity;

            // Set to new rotation
            targetRotation = targetRotation *= Quaternion.Euler(0, yMovement, 0);
        }

        // Select the current zoomlevel based on scroll
        if (Input.mouseScrollDelta.y > 0.0f && currentStep > minZoomSteps)
        {
            currentStep -= 1;
        }
        else if (Input.mouseScrollDelta.y < 0.0f && currentStep < maxZoomSteps)
        {
            currentStep += 1;
        }

        // Keyboard movement of the camera container
        Vector3 cameraTranslation = new Vector3();

        // Fly over the board in all directions
        if (Input.GetKey(KeyCode.W)) cameraTranslation += cameraOffsetTarget.transform.forward;
        if (Input.GetKey(KeyCode.D)) cameraTranslation += cameraOffsetTarget.transform.right;
        if (Input.GetKey(KeyCode.S)) cameraTranslation += cameraOffsetTarget.transform.forward * -1;
        if (Input.GetKey(KeyCode.A)) cameraTranslation += cameraOffsetTarget.transform.right * -1;

        // Rotate Camera with Q & E
        if (Input.GetKey(KeyCode.E)) targetRotation *= Quaternion.Euler(0, 1, 0);
        if (Input.GetKey(KeyCode.Q)) targetRotation *= Quaternion.Euler(0, -1, 0);

        // Add translation to position
        cameraTranslation.Normalize();
        targetPosition += cameraTranslation * speed * Time.deltaTime;

        // Calculate absolute offset based on zoom step
        targetOffset = new Vector3(0,
            zoomOffsetYStep * currentStep,
            zoomOffsetZStep * currentStep);

        // Calculate absolute camera tilt based on zoom step
        targetTilt = Quaternion.Euler(zoomTiltStep * currentStep, 0, 0);

        // Set the target objects
        cameraCenterTarget.transform.localPosition = targetPosition;
        cameraCenterTarget.transform.localRotation = targetRotation;
        cameraOffsetTarget.transform.localPosition = targetOffset;
        mainCameraTarget.transform.localRotation = targetTilt;

        // Get a position above the actual camera position
        Vector3 mainCameraRaycastStart = new Vector3(
            mainCameraTarget.transform.position.x,
            500,
            mainCameraTarget.transform.position.z);

        // Shoot ray down to terrain
        if (Physics.Raycast(
                mainCameraRaycastStart,
                Vector3.down,
                out hit, 1000, LayerMask.GetMask(
                                LayerMask.LayerToName(Constants.worldFloorLayer),
                                LayerMask.LayerToName(Constants.tileLayer))))
        {
            // Check if the camera below the terrain. If so, move up
            if (hit.point.y > targetPosition.y )
            {
                targetPosition.y = hit.point.y;
                cameraCenterTarget.transform.localPosition = targetPosition;
                Debug.Log("Adding height");
            }
        }

        // Calculate the interpolated positions
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * cameraPositionLerpSpeed);
        currentRotation = Quaternion.Lerp(currentRotation, targetRotation , Time.deltaTime * cameraRotationLerpSpeed);
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * cameraPositionLerpSpeed);
        currentTilt = Quaternion.Lerp(currentTilt, targetTilt, Time.deltaTime * cameraRotationLerpSpeed);

        // Move actual camera
        cameraCenter.transform.localPosition = currentPosition;
        cameraCenter.transform.localRotation = currentRotation;
        cameraOffset.transform.localPosition = currentOffset;
        mainCamera.transform.localRotation = currentTilt;
    }

    public void SetCameraRotation(Quaternion cameraRotation)    
    {
        targetRotation = cameraRotation;
    }

    public Quaternion GetCameraRotation()
    {
        return targetRotation;
    }

    public void SetCameraContainerPosition(Vector3 cameraContainerPosition)
    {
        targetPosition = cameraContainerPosition;
    }

    public Vector3 GetCameraContainerPosition()
    {
        return targetPosition;
    }

    public void SetCameraContainerRotation(Quaternion cameraContainerRotation)
    {
        targetRotation = cameraContainerRotation;
    }

    public Quaternion GetCameraContainerRotation()
    {
        return targetRotation;
    }

    public void SetZoomLevel(int zoomLevel)
    {
        currentStep = zoomLevel;
    }

    public int GetZoomLevel()
    {
        return currentStep;
    }
}