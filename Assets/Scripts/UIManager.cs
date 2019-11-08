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
    private Text coinsCount;
    
    private GameObject buttonHouse;
    private GameObject buttonRoad;
    private GameObject buttonDestroy;

    private ConstructionType constructionType;

    private Player currentPlayer;

    private Color borderColorUnselected;
    private Color buildinColorUnavailable;

    private Image buttonHousePictogramImage;
    private Image buttonRoadPictogramImage;
    private Image buttonDestroyPictogramImage;

    private Image buttonHouseBorderImage;
    private Image buttonRoadBorderImage;
    private Image buttonDestroyBorderImage;

    private void Awake()
    {
        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();
        coinsCount = GameObject.Find("Text Coins").GetComponent<Text>();

        buttonHousePictogramImage = GameObject.Find("Button House").transform.Find("Pictogram").GetComponent<Image>();
        buttonRoadPictogramImage = GameObject.Find("Button Road").transform.Find("Pictogram").GetComponent<Image>();
        buttonDestroyPictogramImage = GameObject.Find("Button Destroy").transform.Find("Pictogram").GetComponent<Image>();

        buttonHouseBorderImage = GameObject.Find("Button House").transform.Find("Border").GetComponent<Image>();
        buttonRoadBorderImage = GameObject.Find("Button Road").transform.Find("Border").GetComponent<Image>();
        buttonDestroyBorderImage = GameObject.Find("Button Destroy").transform.Find("Border").GetComponent<Image>();

        borderColorUnselected = new Color(.5f, .5f, .5f, .0f);
    }

    public void UpdateCurrentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
        playername.text = currentPlayer.GetName() + " (" + GameManager.Instance.turnManager.GetGamePhase() + ")";

        buildinColorUnavailable = currentPlayer.GetColor();
        buildinColorUnavailable.a = .5f;

        SetBuildingMode(ConstructionType.None);

        buttonDestroyPictogramImage.color = currentPlayer.GetColor();
    }

    public void UpdateResources(int stone, int wood, int wheat, int coins)
    {
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();
        coinsCount.text = coins.ToString();
        
        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, ConstructionType.House))
        {
            buttonHousePictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonHousePictogramImage.color = buildinColorUnavailable;
        }

        if (GameManager.Instance.buildingManager.HasPlayerEnoughRessourcesToBuild(currentPlayer, ConstructionType.Road))
        {
            buttonRoadPictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonRoadPictogramImage.color = buildinColorUnavailable;
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
                buttonHouseBorderImage.color = currentPlayer.GetColor();
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
                break;

            case ConstructionType.Road:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = currentPlayer.GetColor();
                buttonDestroyBorderImage.color = borderColorUnselected;
                break;

            case ConstructionType.Destroy:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = currentPlayer.GetColor();
                break;

            case ConstructionType.None:
                // Unsets everything when turn changes
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
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
