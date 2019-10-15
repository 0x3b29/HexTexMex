using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    Material materialBuildAllowed;
    Material materialBuildDenied;

    GameObject tileHighlighter;

    Dropdown BuildingSelect;

    // Start is called before the first frame update
    void Start()
    {
        BuildingSelect = GameObject.Find("Dropdown").GetComponent<Dropdown>();

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
                string selected = BuildingSelect.options[BuildingSelect.value].text;

                bool actionAllowed = false;

                if (tile.isFree() && selected.Equals("Road"))
                {
                    actionAllowed = true;
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                    {
                            tile.addRoad();
                        }
                }
                
                if (tile.isFree() && selected.Equals("House"))
                        {
                    actionAllowed = true;
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                            {
                        tile.placeHouse();
                            }
                        }

                if ((tile.isRoad || tile.woodhouse) && selected.Equals("Destroy"))
                        {
                    actionAllowed = true;
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                        {
                        tile.destroyFeature();
                    }
                }

                // If tile and action did not match for any case, show a red marker 
                if (!actionAllowed)
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
