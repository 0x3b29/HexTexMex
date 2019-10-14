using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Material materialBuildAllowed;
    Material materialBuildDenied;

    GameObject tileHighlighter;

    // Start is called before the first frame update
    void Start()
    {
        materialBuildAllowed = Resources.Load(Constants.materialsFolder + "BuildAllowed", typeof(Material)) as Material;
        materialBuildDenied = Resources.Load(Constants.materialsFolder + "BuildDenied", typeof(Material)) as Material;
        
        tileHighlighter = Instantiate(Resources.Load(Constants.prefabFolder + "Torus Parent") as GameObject, new Vector3(), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
            if (hit.transform.gameObject.name.Equals("Hexagon"))
            {
                tileHighlighter.SetActive(true);
                tileHighlighter.transform.position = hit.transform.gameObject.transform.position;

                Tile tile = hit.transform.gameObject.transform.parent.GetComponent<Tile>();

                if (tile.isFree())
                {
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                    {
                GameObject woodHouse = Instantiate(Resources.Load(Constants.prefabFolder + "Woodhouse Parent") as GameObject, hit.transform.position, Quaternion.identity);
                woodHouse.transform.parent = hit.transform.gameObject.transform;

                        Quaternion rotation = woodHouse.transform.rotation;
                        rotation.eulerAngles = new Vector3(0,Random.Range(0,360),0);
                        woodHouse.transform.rotation = rotation;

                woodHouse.name = "woodHouse";
                        tile.isWoodhouse = true;
                    }
                }
                else
                {
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildDenied;
                }
            }
            }
        else
        {
            tileHighlighter.SetActive(false);
        }
    }
}
