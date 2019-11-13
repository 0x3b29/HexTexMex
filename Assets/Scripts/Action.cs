using UnityEngine;
using UnityEngine.UI;

public class Action
{
    private ActionType actionType;
    private string name;
    
    // The cost of the action instance
    private int stoneCost;
    private int woodCost;
    private int wheatCost;
    private int dragonCost;

    private Image buttonPictogramImage;
    private Image buttonBorderImage;

    public Action (ActionType actionType, string name, string buttonName, int stoneCost, int woodCost, int wheatCost)
    {
        this.actionType = actionType;
        this.name = name;
        this.stoneCost = stoneCost;
        this.woodCost = woodCost;
        this.wheatCost = wheatCost;

        FindButtonImages(buttonName);
    }

    public Action(ActionType actionType, string name, string buttonName, int dragonCost)
    {
        this.actionType = actionType;
        this.name = name;
        this.dragonCost = dragonCost;

        FindButtonImages(buttonName);
    }
    
    private void FindButtonImages(string buttonName)
    {
        buttonPictogramImage = GameObject.Find(buttonName).transform.Find("Pictogram").GetComponent<Image>();
        buttonBorderImage = GameObject.Find(buttonName).transform.Find("Border").GetComponent<Image>();
    }

    public ActionType GetActionType()
    {
        return actionType;
    }

    public string GetName()
    {
        return this.name;
    }

    public int GetStoneCost()
    {
        return stoneCost;
    }

    public int GetWoodCost()
    {
        return woodCost;
    }

    public int GetWheatCost()
    {
        return wheatCost;
    }

    public int GetDragonCost()
    {
        return dragonCost;
    }

    public Image GetButtonPictogramImage()
    {
        return buttonPictogramImage;
    }

    public Image GetButtonBorderImage()
    {
        return buttonBorderImage;
    }
}