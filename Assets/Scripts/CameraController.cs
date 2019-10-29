using UnityEngine;

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

            // Set to new rotation
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + xMovement, transform.eulerAngles.y + yMovement, 0);
        }

        // Keyboard movement
        Vector3 keyboardMovement = new Vector3();
        
        // Zoom down / up with the scroll wheel
        if (Input.mouseScrollDelta.y > 0.0f) keyboardMovement += new Vector3(0, -5, +10);
        if (Input.mouseScrollDelta.y < 0.0f) keyboardMovement += new Vector3(0, +5, -10);
        
        // Fly over the board in all directions
        if (Input.GetKey(KeyCode.W)) keyboardMovement += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.A)) keyboardMovement += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) keyboardMovement += new Vector3(0, 0, -1);        
        if (Input.GetKey(KeyCode.D)) keyboardMovement += new Vector3(1, 0, 0);
        
        // Rotate Camera with Q & E
        if (Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.up * 100f * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q)) transform.Rotate(Vector3.down * 100f * Time.deltaTime);

        // Add movement 
        transform.Translate(keyboardMovement * speed * Time.deltaTime);
    }
}