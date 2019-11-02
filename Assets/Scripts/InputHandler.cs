using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
        // Get selected building type
        BuildingMode currentBuildingMode = GameManager.Instance.UIManager.getBuildingMode();

        // Do not perform ray casts through buttons and other GUI objects
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
        {
            return;
        }

        // Capture a screenshot with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("Screenshots/" + System.DateTime.Now.ToString("dd-MM-yy_hh-mm-ss") + ".png");
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100, 1 << Constants.tileLayer))
        {
            if (hit.transform.gameObject.name.Equals("Hexagon"))
            {
                tileHighlighter.SetActive(true);
                tileHighlighter.transform.position = hit.transform.gameObject.transform.position;

                Tile tile = hit.transform.gameObject.transform.parent.GetComponent<Tile>();

                bool actionAllowed = false;

                if (tile.isFree() && currentBuildingMode.Equals(BuildingMode.Road))
                {
                    actionAllowed = true;
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                    {
                            tile.addRoad();
                        }
                }
                
                if (tile.isFree() && currentBuildingMode.Equals(BuildingMode.House))
                        {
                    actionAllowed = true;
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                            {
                        tile.placeHouse();
                            }
                        }

                if ((tile.isRoad || tile.woodhouse) && currentBuildingMode.Equals(BuildingMode.Destroy))
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
