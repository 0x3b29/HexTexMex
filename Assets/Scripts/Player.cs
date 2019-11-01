using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Color color;

    private int wood;
    private int stone;
    private int wheat;

    private List<Tile> tilesWithHouses;

    public Player (Color color, int wood, int stone, int wheat)
    {
        tilesWithHouses = new List<Tile>();

        this.color = color;
        this.wood = wood;
        this.stone = stone;
        this.wheat = wheat;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
