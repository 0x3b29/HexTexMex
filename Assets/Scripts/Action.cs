public class Action
{
    private ActionType actionType;
    private string name;
    
    // The cost of the action instance
    private int stoneCost;
    private int woodCost;
    private int wheatCost;
    private int dragonCost;
    
    public Action (ActionType actionType, string name, int stoneCost, int woodCost, int wheatCost)
    {
        this.actionType = actionType;
        this.name = name;
        this.stoneCost = stoneCost;
        this.woodCost = woodCost;
        this.wheatCost = wheatCost;
    }

    public Action(ActionType actionType, string name, int dragonCost)
    {
        this.actionType = actionType;
        this.name = name;
        this.dragonCost = dragonCost;
    }
    
    public ActionType GetConstructionType()
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
}