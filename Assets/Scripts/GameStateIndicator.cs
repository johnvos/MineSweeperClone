using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateIndicator : MonoBehaviour
{
    public Sprite Normal;
    public Sprite Dead;
    public Sprite Win;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = Normal;
    }

    public void GameOver(bool win) {
        if (win) {
            sr.sprite = Win;
        } else {
            sr.sprite = Dead;
        }
    }
}
