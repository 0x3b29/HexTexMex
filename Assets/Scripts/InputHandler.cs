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
        BuildingManager buildingManager = GameManager.Instance.buildingManager;
        Player currentPlayer = GameManager.Instance.turnManager.GetCurrentPlayer();
        UIManager uiManager = GameManager.Instance.uiManager;

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

                // Let the buildingManager decide if the selected action on the current tile can be performed by the current player
                if (buildingManager.IsActionAllowed(tile, currentPlayer))
                {
                    // If the buildmanager allows the current action on selected tile, show a green marker 
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                    {
                        // If the player clickes on the tile, the currently selected action will be executed
                        buildingManager.PerformAction(tile, currentPlayer);
                        uiManager.UpdateRessources(currentPlayer.GetStone(), currentPlayer.GetWood(), currentPlayer.GetWheat());
                    }
                }
                else
                {
                    // If the buildmanager declines the current action on selected tile, show a red marker 
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
