using UnityEngine;
using System.Collections;

// Inspired by https://gist.github.com/JohannesMP/e15fe61386d4381d4441c3c324d96c56
public class CameraController : MonoBehaviour {
      
    float speed = 25.0f;
    float mouseSensitivity = 0.2f;
    private Vector3 oldMousePosition; 
     
    void Update ()
    {
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
            float xMovement = -mouseDelta.y * mouseSensitivity;
            float yMovement = mouseDelta.x * mouseSensitivity;

            // Add movement 
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + xMovement, transform.eulerAngles.y + yMovement, 0);
        }

        // Keyboard movement
        Vector3 keyboardMovement = new Vector3();

        if (Input.GetKey(KeyCode.W)) keyboardMovement += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.A)) keyboardMovement += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) keyboardMovement += new Vector3(0, 0, -1);        
        if (Input.GetKey(KeyCode.D)) keyboardMovement += new Vector3(1, 0, 0);

        transform.Translate(keyboardMovement * speed * Time.deltaTime);      
    }
}