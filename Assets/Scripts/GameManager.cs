using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public Mouse mouse;

    public int width;
    public int height;
    public int minecount;

    bool gameEnded;

    private void Awake() {
        gameEnded = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResizeCamera();

        board.SetupBoard(width, height, minecount);
        board.SpawnBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded) {
            switch (board.GetCurrentGameState()) {
                case 1:
                    Debug.Log("YOU WON");
                    gameEnded = true;
                    mouse.GameOver();
                    break;
                case 99:
                    Debug.Log("YOU LOST");
                    gameEnded = true;
                    mouse.GameOver();
                    break;
                default:
                    break;
            }
        }

    }


    void ResizeCamera() {
        int cameraSize = 0;
        if(width > height) {
            cameraSize = width / 2 + width % 2 + 2;
        } else {
            cameraSize = height / 2 + height % 2 + 2;
        }
        Camera.main.orthographicSize = cameraSize;
    }
}
