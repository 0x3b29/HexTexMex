using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHelper : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Capture a screenshot with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("Screenshots/" + System.DateTime.Now.ToString("dd-MM-yy_hh-mm-ss") + ".png");
            Debug.Log("Captured : " + "Screenshots / " + System.DateTime.Now.ToString("dd - MM - yy_hh - mm - ss") + ".png");
        }
    }
}
