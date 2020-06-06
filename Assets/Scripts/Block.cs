using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    public Sprite BlockNormal;
    public Sprite BlockPressed;
    public Sprite BlockOpen;

    bool open;

    int x;
    int y;

    SpriteRenderer sr;
    Board parent;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        x = -1;
        y = -1;
        open = false;
    }

    private void Update() {
        if (open) { //if block is opened already, don't react to clicks
            if(sr.sprite != BlockOpen) sr.sprite = BlockOpen;
            return;
        }

        if(sr.sprite == BlockPressed && !open) {
            StartCoroutine(ComingUp());
        }
    }

    public void MouseLeft() {
        if (open) return;
        sr.sprite = BlockPressed;
        StopAllCoroutines();
    }

    public void MouseLeftUp() {
        if (open) return;
        Debug.Log("LeftUp!");
        //tell parent it has been opened
        parent.ClickLeft(x, y);

        StopAllCoroutines();
    }

    public void MouseRightDown() {
        if (open) return;
        Debug.Log("RightDown!");

        //tell parent it has been right clicked
        parent.ClickRight(x, y);
    }

    public void Open() {
        open = true;
        StopAllCoroutines();
    }

    public void Close() {
        open = false;
        StopAllCoroutines();
    }

    public bool IsOpen() {
        return open;
    }

    private IEnumerator ComingUp() {
        yield return new WaitForSeconds(0.2f);
        sr.sprite = BlockNormal;
    }

    public void SetParent(Board par, int x, int y) {
        this.x = x;
        this.y = y;
        parent = par;
    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }
}
