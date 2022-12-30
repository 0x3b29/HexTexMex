using System;
using UnityEngine;

public struct CameraSetup
{
    public Vector3 position;
    public float rotation;
    public Vector3 offset;
    public Quaternion tilt;

    public int zoomLevel;
}

// Inspired by https://gist.github.com/JohannesMP/e15fe61386d4381d4441c3c324d96c56
public class CameraController : MonoBehaviour {
      
    public float speed = 25.0f;
    public float mouseSensitivity = 0.2f;

    public float cameraRotationLerpSpeed = 3f;
    public float cameraPositionLerpSpeed = 3f;

    public int minZoomLevels = 3;
    public int maxZoomLevels = 10;
    
    public float zoomTiltStep = 2f;
    public float zoomOffsetYStep = 5f;
    public float zoomOffsetZStep = -10f;

    public float rotationToAddEveryFrame = 0;

    private Vector3 oldMousePosition;

    public GameObject cameraCenter;
    public GameObject cameraOffset;
    public GameObject mainCamera;

    public GameObject cameraCenterTarget;
    public GameObject cameraOffsetTarget;
    public GameObject mainCameraTarget;

    public GameObject cameraHitPoint;

    private CameraSetup currentCameraSetup;
    private CameraSetup targetCameraSetup;

    private void Awake()
    {
        currentCameraSetup.rotation = cameraCenter.transform.rotation.eulerAngles.y;
        targetCameraSetup.rotation = cameraCenter.transform.rotation.eulerAngles.y;

        currentCameraSetup.position = cameraCenter.transform.position;
        targetCameraSetup.position = cameraCenter.transform.position;

        targetCameraSetup.zoomLevel = 8;
    }

    void Update ()
    {
        targetCameraSetup.rotation += rotationToAddEveryFrame;

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
            targetCameraSetup.position.y = hit.point.y;
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
            targetCameraSetup.rotation += yMovement;
        }

        // Select the current zoomlevel based on scroll
        if (Input.mouseScrollDelta.y > 0.0f && targetCameraSetup.zoomLevel > minZoomLevels)
        {
            targetCameraSetup.zoomLevel -= 1;
        }
        else if (Input.mouseScrollDelta.y < 0.0f && targetCameraSetup.zoomLevel < maxZoomLevels)
        {
            targetCameraSetup.zoomLevel += 1;
        }

        // Keyboard movement of the camera container
        Vector3 cameraTranslation = new Vector3();

        // Fly over the board in all directions
        if (Input.GetKey(KeyCode.W)) cameraTranslation += cameraOffsetTarget.transform.forward;
        if (Input.GetKey(KeyCode.D)) cameraTranslation += cameraOffsetTarget.transform.right;
        if (Input.GetKey(KeyCode.S)) cameraTranslation += cameraOffsetTarget.transform.forward * -1;
        if (Input.GetKey(KeyCode.A)) cameraTranslation += cameraOffsetTarget.transform.right * -1;

        // Rotate Camera with Q & E
        if (Input.GetKey(KeyCode.E)) targetCameraSetup.rotation += 1;
        if (Input.GetKey(KeyCode.Q)) targetCameraSetup.rotation -= 1;

        // Add translation to position
        cameraTranslation.Normalize();
        targetCameraSetup.position += cameraTranslation * speed * Time.deltaTime;

        // Calculate absolute offset based on zoom step
        targetCameraSetup.offset = new Vector3(0,
            zoomOffsetYStep * targetCameraSetup.zoomLevel,
            zoomOffsetZStep * targetCameraSetup.zoomLevel);

        // Calculate absolute camera tilt based on zoom step
        targetCameraSetup.tilt = Quaternion.Euler(zoomTiltStep * targetCameraSetup.zoomLevel, 0, 0);

        // Set the target objects
        cameraCenterTarget.transform.localPosition = targetCameraSetup.position;
        cameraCenterTarget.transform.localRotation = Quaternion.Euler(0, targetCameraSetup.rotation, 0);
        cameraOffsetTarget.transform.localPosition = targetCameraSetup.offset;
        mainCameraTarget.transform.localRotation = targetCameraSetup.tilt;

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
            // Thme small offset to avoid float mismatch for almost equal values
            if (hit.point.y > targetCameraSetup.position.y + 0.001f)
            {
                targetCameraSetup.position.y = hit.point.y;
                cameraCenterTarget.transform.localPosition = targetCameraSetup.position;
            }
        }

        // Calculate the interpolated positions
        currentCameraSetup.position = Vector3.Lerp(currentCameraSetup.position, targetCameraSetup.position, Time.deltaTime * cameraPositionLerpSpeed);
        currentCameraSetup.rotation = Mathf.Lerp(currentCameraSetup.rotation, targetCameraSetup.rotation , Time.deltaTime * cameraRotationLerpSpeed);
        currentCameraSetup.offset = Vector3.Lerp(currentCameraSetup.offset, targetCameraSetup.offset, Time.deltaTime * cameraPositionLerpSpeed);
        currentCameraSetup.tilt = Quaternion.Lerp(currentCameraSetup.tilt, targetCameraSetup.tilt, Time.deltaTime * cameraRotationLerpSpeed);

        // Move actual camera
        cameraCenter.transform.localPosition = currentCameraSetup.position;
        cameraCenter.transform.localRotation = Quaternion.Euler(0, currentCameraSetup.rotation, 0);
        cameraOffset.transform.localPosition = currentCameraSetup.offset;
        mainCamera.transform.localRotation = currentCameraSetup.tilt;
    }

    public void SetTargetCameraSetup(CameraSetup cameraSetup)    
    {
        cameraSetup.rotation = cameraSetup.rotation % 360f;
        targetCameraSetup = cameraSetup;
    }

    public void NormalizeCameraRotation()
    {
        targetCameraSetup.rotation = targetCameraSetup.rotation % 360f;

        if (targetCameraSetup.rotation < 0)
            targetCameraSetup.rotation += 360f;

        currentCameraSetup.rotation = targetCameraSetup.rotation;
    }

    public CameraSetup GetTargetCameraSetup()
    {
        return targetCameraSetup;
    }
}