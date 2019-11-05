using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI references
    private Text playername;
    private Text stoneCount;
    private Text woodCount;
    private Text wheatCount;

    private GameObject buttonHouse;
    private GameObject buttonRoad;
    private GameObject buttonDestroy;

    private BuildingMode buildingMode;

    private Player currentPlayer;

    private Color borderColorUnselected;
    private Color buildinColorUnavailable;

    private void Awake()
    {
        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();

        buttonHouse = GameObject.Find("Button House");
        buttonRoad = GameObject.Find("Button Road");
        buttonDestroy = GameObject.Find("Button Destroy");

        borderColorUnselected = new Color(.5f, .5f, .5f, .0f);
        
    }

    public void UpdateCurrentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
        playername.text = currentPlayer.GetName();

        buildinColorUnavailable = currentPlayer.GetColor();
        buildinColorUnavailable.a = .5f;

        setBuildingModeNone();

        buttonDestroy.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
    }

    public void UpdateRessources(int stone, int wood, int wheat)
    {
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();

        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, GameManager.Instance.buildingManager.getWoodhouse()))
        {
            buttonHouse.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
        }
        else
        {
            buttonHouse.transform.Find("Pictogram").GetComponent<Image>().color = buildinColorUnavailable;
        }

        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, GameManager.Instance.buildingManager.getRoad()))
        {
            buttonRoad.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
        }
        else
        {
            buttonRoad.transform.Find("Pictogram").GetComponent<Image>().color = buildinColorUnavailable;
        }
    }

    // Function referenced in UI
    public void setBuildingModeNone()
    {
        buildingMode = BuildingMode.None;

        buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
    }

    // Function referenced in UI
    public void setBuildingModeHouse()
    {
        buildingMode = BuildingMode.House;

        buttonHouse.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
        buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
    }

    // Function referenced in UI
    public void setBuildingModeRoad()
    {
        buildingMode = BuildingMode.Road;

        buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonRoad.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
        buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
    }

    // Function referenced in UI
    public void setBuildingModeDestroy()
    {
        buildingMode = BuildingMode.Destroy;

        buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
        buttonDestroy.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
    }

    public BuildingMode getBuildingMode()
    {
        return buildingMode;
    }
}
