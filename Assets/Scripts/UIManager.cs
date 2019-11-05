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

    private ConstructionType constructionType;

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

        SetBuildingMode(ConstructionType.None);

        buttonDestroy.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
    }

    public void UpdateRessources(int stone, int wood, int wheat)
    {
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();

        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, ConstructionType.House))
        {
            buttonHouse.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
        }
        else
        {
            buttonHouse.transform.Find("Pictogram").GetComponent<Image>().color = buildinColorUnavailable;
        }

        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, ConstructionType.Road))
        {
            buttonRoad.transform.Find("Pictogram").GetComponent<Image>().color = currentPlayer.GetColor();
        }
        else
        {
            buttonRoad.transform.Find("Pictogram").GetComponent<Image>().color = buildinColorUnavailable;
        }
    }

    // Function referenced in UI
    public void SetBuildingMode(string buildingModeAsString)
    {
        ConstructionType newBuildingMode;
        // Ugly hack because of https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/
        if (!System.Enum.TryParse(buildingModeAsString, out newBuildingMode))
        {
            Debug.LogWarning("UIManager: buildingModeAsString '" + buildingModeAsString + "' could not be parsed");
            return;
        }

        SetBuildingMode(newBuildingMode);
    }

    // Function referenced in code
    public void SetBuildingMode(ConstructionType constructionType)
    {
        this.constructionType = constructionType;
        GameManager.Instance.buildingManager.SetBuildingMode(constructionType);

        switch (constructionType)
        {
            case ConstructionType.House:
                buttonHouse.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
                buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                break;

            case ConstructionType.Road:
                buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonRoad.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
                buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                break;

            case ConstructionType.Destroy:
                buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonDestroy.transform.Find("Border").GetComponent<Image>().color = currentPlayer.GetColor();
                break;

            case ConstructionType.None:
                // Unsets everything when turn changes
                buttonHouse.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonRoad.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                buttonDestroy.transform.Find("Border").GetComponent<Image>().color = borderColorUnselected;
                break;

            default:
                break;
        }
    }

    public ConstructionType GetConstructionType()
    {
        return constructionType;
    }
}
