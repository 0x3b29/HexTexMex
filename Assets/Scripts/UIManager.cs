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

    private Image buttonHouseImage;
    private Image buttonRoadImage;
    private Image buttonDestroyImage;

    private BuildingMode buildingMode;

    private void Awake()
    {
        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();

        buttonHouseImage = GameObject.Find("Button House").GetComponent<Image>();
        buttonRoadImage = GameObject.Find("Button Road").GetComponent<Image>();
        buttonDestroyImage = GameObject.Find("Button Destroy").GetComponent<Image>();
    }

    public void UpdatePlayername(string name)
    {
        playername.text = name;
    }

    public void UpdateRessources(int stone, int wood, int wheat)
    {
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();
    }

    // Function referenced in UI
    public void setBuildingModeHouse()
    {
        buildingMode = BuildingMode.House;
    }

    // Function referenced in UI
    public void setBuildingModeRoad()
    {
        buildingMode = BuildingMode.Road;
    }

    // Function referenced in UI
    public void setBuildingModeDestroy()
    {
        buildingMode = BuildingMode.Destroy;
    }

    public void SetButtonColor(Color color)
    {
        buttonHouseImage.color = color;
        buttonRoadImage.color = color;
        buttonDestroyImage.color = color;
    }

    public BuildingMode getBuildingMode()
    {
        return buildingMode;
    }
}
