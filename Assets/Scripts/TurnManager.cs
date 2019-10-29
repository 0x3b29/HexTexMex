using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private static bool _player1Turn = true;
    
    private static float player1Wood = 2f;
    private static float player1Stone = 2f;
    private static float player1Wheat = 1f;
    private static float player2Wood = 2f;
    private static float player2Stone = 2f;
    private static float player2Wheat = 1f;
    
    public Text player1;
    public Text player1Resources;
    public Text player2;
    public Text player2Resources;
    public Text turnIndicator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player1Resources.text = "Wood " + player1Wood + " - Stone " + player1Stone + " - Wheat " + player1Wheat;
        player2Resources.text = "Wood " + player2Wood + " - Stone " + player2Stone + " - Wheat " + player2Wheat;
    }

    public void endTurn()
    {
        // Deselect single button on GUI...
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject (null);
        
        // End the turn
        _player1Turn = !_player1Turn;
        
        // Adapt the turn indicator
        if (_player1Turn)
        {
            turnIndicator.text = "It is your turn " + player1.text + "!";            
        }
        else
        {
            turnIndicator.text = "It is your turn " + player2.text + "!";
        }
        
        // Give everybody some additional resources independent of the current villages
        player1Wood += 1f;
        player1Stone += 1f;
        player1Wheat += 1f;
        player2Wood += 1f;
        player2Stone += 1f;
        player2Wheat += 1f;
    }

    public static bool isPlayer1Turn()
    {
        return _player1Turn;
    }
}
