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

    ActionManager actionManager;
    private ActionType actionType;

    private Player currentPlayer;

    private Color borderColorUnselected;
    private Color colorActionUnavailable;

    public void Initialize()
    {
        actionManager = GameManager.Instance.actionManager;

        playername = GameObject.Find("Text Playername").GetComponent<Text>();
        stoneCount = GameObject.Find("Text Stone").GetComponent<Text>();
        woodCount = GameObject.Find("Text Wood").GetComponent<Text>();
        wheatCount = GameObject.Find("Text Wheat").GetComponent<Text>();
        coinsCount = GameObject.Find("Text Coins").GetComponent<Text>();
        winnerLabel = GameObject.Find("Winner").GetComponent<Text>();

        borderColorUnselected = new Color(.5f, .5f, .5f, .0f);
    }

    public void UpdateCurrentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
        playername.text = currentPlayer.GetName() + " (" + GameManager.Instance.turnManager.GetGamePhase() + ")";

        colorActionUnavailable = currentPlayer.GetColor();
        colorActionUnavailable.a = .5f;

        SetBuildingMode(currentPlayer.GetSelectedActionType());
    }

    public void UpdateResources(int stone, int wood, int wheat, int coins)
    {
        // Update resource labels
        stoneCount.text = stone.ToString();
        woodCount.text = wood.ToString();
        wheatCount.text = wheat.ToString();
        coinsCount.text = coins.ToString();
        
        // Enable or disable buttons depending on resources
        foreach(ActionType actionType in actionManager.GetActionTypes())
        {
            if (GameManager.Instance.actionManager.IsActionAllowed(currentPlayer, actionType))
                actionManager.GetActions().Find(action => action.GetActionType().Equals(actionType)).GetButtonPictogramImage().color = currentPlayer.GetColor();
            else
                actionManager.GetActions().Find(action => action.GetActionType().Equals(actionType)).GetButtonPictogramImage().color = colorActionUnavailable;
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
        GameManager.Instance.turnManager.GetCurrentPlayer().SetSelectedActionType(actionType);

        // Iterate over all actions and set bordercolor for passed actiontype
        foreach (Action action in actionManager.GetActions())
        {
            if (action.GetActionType() == actionType)
                action.GetButtonBorderImage().color = currentPlayer.GetColor();
            else
                action.GetButtonBorderImage().color = borderColorUnselected;
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
