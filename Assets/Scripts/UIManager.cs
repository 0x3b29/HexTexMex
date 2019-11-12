using System;
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
    private Text winnerLabel;
    
    private GameObject buttonHouse;
    private GameObject buttonRoad;
    private GameObject buttonDestroy;

    private ActionType actionType;

    private Player currentPlayer;

    private Color borderColorUnselected;
    private Color buildinColorUnavailable;

    private Image buttonHousePictogramImage;
    private Image buttonRoadPictogramImage;
    private Image buttonTraderPictogramImage;
    private Image buttonDestroyPictogramImage;
    private Image buttonDragonPictogramImage;

    private Image buttonHouseBorderImage;
    private Image buttonRoadBorderImage;
    private Image buttonTraderBorderImage;
    private Image buttonDestroyBorderImage;
    private Image buttonDragonBorderImage;

    private void Awake()
    {
        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();
        coinsCount = GameObject.Find("Text Coins").GetComponent<Text>();
        winnerLabel = GameObject.Find("Winner").GetComponent<Text>();
        
        buttonHousePictogramImage = GameObject.Find("Button House").transform.Find("Pictogram").GetComponent<Image>();
        buttonRoadPictogramImage = GameObject.Find("Button Road").transform.Find("Pictogram").GetComponent<Image>();
        buttonTraderPictogramImage = GameObject.Find("Button Trader").transform.Find("Pictogram").GetComponent<Image>();
        buttonDestroyPictogramImage = GameObject.Find("Button Destroy").transform.Find("Pictogram").GetComponent<Image>();
        buttonDragonPictogramImage = GameObject.Find("Button Dragon").transform.Find("Pictogram").GetComponent<Image>();

        buttonHouseBorderImage = GameObject.Find("Button House").transform.Find("Border").GetComponent<Image>();
        buttonRoadBorderImage = GameObject.Find("Button Road").transform.Find("Border").GetComponent<Image>();
        buttonTraderBorderImage = GameObject.Find("Button Trader").transform.Find("Border").GetComponent<Image>();
        buttonDestroyBorderImage = GameObject.Find("Button Destroy").transform.Find("Border").GetComponent<Image>();
        buttonDragonBorderImage = GameObject.Find("Button Dragon").transform.Find("Border").GetComponent<Image>();

        borderColorUnselected = new Color(.5f, .5f, .5f, .0f);
    }

    public void UpdateCurrentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
        playername.text = currentPlayer.GetName() + " (" + GameManager.Instance.turnManager.GetGamePhase() + ")";

        buildinColorUnavailable = currentPlayer.GetColor();
        buildinColorUnavailable.a = .5f;

        SetBuildingMode(ActionType.None);

        buttonDestroyPictogramImage.color = currentPlayer.GetColor();
    }

    public void UpdateResources(int stone, int wood, int wheat, int coins)
    {
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();
        coinsCount.text = coins.ToString();
        
        if (GameManager.Instance.actionManager.IsActionAllowed(currentPlayer, ActionType.House))
        {
            buttonHousePictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonHousePictogramImage.color = buildinColorUnavailable;
        }

        if (GameManager.Instance.actionManager.IsActionAllowed(currentPlayer, ActionType.Road))
        {
            buttonRoadPictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonRoadPictogramImage.color = buildinColorUnavailable;
        }

        if (GameManager.Instance.actionManager.IsActionAllowed(currentPlayer, ActionType.Trader))
        {
            buttonTraderPictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonTraderPictogramImage.color = buildinColorUnavailable;
        }
        
        if (GameManager.Instance.actionManager.IsActionAllowed(currentPlayer, ActionType.Dragon))
        {
            buttonDragonPictogramImage.color = currentPlayer.GetColor();
        }
        else
        {
            buttonDragonPictogramImage.color = buildinColorUnavailable;
        }
    }

    // Function referenced in UI
    public void SetBuildingMode(string buildingModeAsString)
    {
        ActionType newBuildingMode;
        // Ugly hack because of https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/
        if (!System.Enum.TryParse(buildingModeAsString, out newBuildingMode))
        {
            Debug.LogWarning("UIManager: buildingModeAsString '" + buildingModeAsString + "' could not be parsed");
            return;
        }

        SetBuildingMode(newBuildingMode);
    }

    // Function referenced in code
    public void SetBuildingMode(ActionType actionType)
    {
        this.actionType = actionType;
        GameManager.Instance.actionManager.SetBuildingMode(actionType);

        switch (actionType)
        {
            case ActionType.House:
                buttonHouseBorderImage.color = currentPlayer.GetColor();
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonTraderBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
                buttonDragonBorderImage.color = borderColorUnselected;
                break;

            case ActionType.Road:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = currentPlayer.GetColor();
                buttonTraderBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
                buttonDragonBorderImage.color = borderColorUnselected;
                break;

            case ActionType.Trader:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonTraderBorderImage.color = currentPlayer.GetColor();
                buttonDestroyBorderImage.color = borderColorUnselected;
                buttonDragonBorderImage.color = borderColorUnselected;
                break;

            case ActionType.Destroy:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonTraderBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = currentPlayer.GetColor();
                buttonDragonBorderImage.color = borderColorUnselected;
                break;

            case ActionType.Dragon:
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonTraderBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
                buttonDragonBorderImage.color = currentPlayer.GetColor();
                break;
            
            case ActionType.None:
                // Unsets everything when turn changes
                buttonHouseBorderImage.color = borderColorUnselected;
                buttonRoadBorderImage.color = borderColorUnselected;
                buttonTraderBorderImage.color = borderColorUnselected;
                buttonDestroyBorderImage.color = borderColorUnselected;
                buttonDragonBorderImage.color = borderColorUnselected;
                break;

            default:
                break;
        }
    }

    public void ShowWinnerLabel(String playerName)
    {
        winnerLabel.text = playerName + " is the Winner!";
    }
    
    public ActionType GetConstructionType()
    {
        return actionType;
    }
}
