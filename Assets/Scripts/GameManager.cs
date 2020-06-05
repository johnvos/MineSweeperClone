using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public Mouse mouse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (board.GetCurrentGameState()) {
            case 1:
                Debug.Log("YOU WON");
                break;
            case 99:
                Debug.Log("YOU LOST");
                break;
            default:
                break;
        }
    }
}
