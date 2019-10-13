using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.Log("Mouse down");

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);

                GameObject woodHouse = Instantiate(Resources.Load("Woodhouse Parent") as GameObject, hit.transform.position, Quaternion.identity);
                woodHouse.transform.parent = hit.transform.gameObject.transform;
                woodHouse.name = "woodHouse";

            }
        }
    }
}
