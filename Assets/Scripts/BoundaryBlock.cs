using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryBlock : MonoBehaviour
{
    public Sprite T;
    public Sprite B;
    public Sprite L;
    public Sprite R;
    public Sprite TR;
    public Sprite TL;
    public Sprite BR;
    public Sprite BL;

    // position indicates which direction this boundary block is
    //  5 = no, 7 = TL, 8 = T, 9 = TR, 4 = L, 6 = R, 1 = BL, 2 = B, 3 = BR
    int position;

    SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        position = 0;
    }

    public void SetPosition(int pos) {
        position = pos;
        switch (position) {
            case 7:
                sr.sprite = TL;
                break;
            case 8:
                sr.sprite = T;
                break;
            case 9:
                sr.sprite = TR;
                break;
            case 4:
                sr.sprite = L;
                break;
            case 6:
                sr.sprite = R;
                break;
            case 1:
                sr.sprite = BL;
                break;
            case 2:
                sr.sprite = B;
                break;
            case 3:
                sr.sprite = BR;
                break;
            default:
                break;
        }
    }
}
