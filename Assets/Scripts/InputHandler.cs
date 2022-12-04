using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    Material materialBuildAllowed;
    Material materialBuildDenied;

    GameObject tileHighlighter;

    private UIManager uiManager;
    private TurnManager turnManager;
    private ActionManager actionManager;

    bool initialized = false;

    public void Initialize()
    {
        materialBuildAllowed = Resources.Load(Constants.materialsFolder + "BuildAllowed", typeof(Material)) as Material;
        materialBuildDenied = Resources.Load(Constants.materialsFolder + "BuildDenied", typeof(Material)) as Material;

        tileHighlighter = Instantiate(Resources.Load(Constants.prefabFolder + "Torus Parent") as GameObject, new Vector3(), Quaternion.identity);

        uiManager = GameManager.Instance.uiManager;
        turnManager = GameManager.Instance.turnManager;
        actionManager = GameManager.Instance.actionManager;

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
            return;

        Player currentPlayer = turnManager.GetCurrentPlayer();

        // Capture a screenshot with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("Screenshots/" + System.DateTime.Now.ToString("dd-MM-yy_hh-mm-ss") + ".png");
        }

        // Hotkeys for menu
        if (Input.GetKeyDown(KeyCode.Alpha1))
            uiManager.SetActionType(ActionType.House);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            uiManager.SetActionType(ActionType.Road);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            uiManager.SetActionType(ActionType.Trader);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            uiManager.SetActionType(ActionType.Dragon);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            uiManager.SetActionType(ActionType.Destroy);

        if (Input.GetKeyDown(KeyCode.Return))
            turnManager.EndTurn();

        // Do not perform ray casts through buttons and other GUI objects
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
        {
            return;
        }

        // Send ray through everything to get the tile the player is pointing at
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100, 1 << Constants.tileLayer))
        {
            if (hit.transform.gameObject.TryGetComponent<TileManager>(out TileManager tileManager))
            {
                tileHighlighter.SetActive(true);
                tileHighlighter.transform.position = hit.transform.gameObject.transform.position;

                // Let the buildingManager decide if the selected action on the current tile can be performed by the current player
                if (actionManager.IsCurrentActionAllowedOnTile(tileManager, currentPlayer))
                {
                    // If the buildmanager allows the current action on selected tile, show a green marker 
                    tileHighlighter.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialBuildAllowed;

                    if (Input.GetMouseButtonDown(0))
                    {
                        // If the player clicks on the tile, the currently selected action will be executed
                        actionManager.PerformAction(tileManager, currentPlayer);
                        uiManager.UpdateResources(
                            currentPlayer.GetStone(), 
                            currentPlayer.GetWood(), 
                            currentPlayer.GetWheat(),
                            currentPlayer.GetCoins());
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
