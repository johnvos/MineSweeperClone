using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour
{
    public List<Sprite> CountSprites;
    public Sprite Flag;
    public Sprite Cross;
    public Sprite Mine;

    State currentState;

    SpriteRenderer sr;

    int neighborMineCount;

    bool open;

    public enum State
    {
        None,
        Count,
        Flag,
        Cross,
        Mine
    }

    private void Awake() {
        currentState = State.None;
        sr = GetComponent<SpriteRenderer>();
        neighborMineCount = -1;
        open = false;
    }

    public void SetInfo(State state) {
        currentState = state;
    }
    private void Update() {
        switch (currentState) {
            case State.None:
                sr.sprite = null;
                break;
            case State.Count:
                if (!open) break;
                if (neighborMineCount == -1) break;
                sr.sprite = CountSprites[neighborMineCount-1];
                break;
            case State.Flag:
                sr.sprite = Flag;
                break;
            case State.Cross:
                sr.sprite = Cross;
                break;
            case State.Mine:
                if (!open) break;
                sr.sprite = Mine;
                break;
            default:
                sr.sprite = null;
                break;
        }
    }

    public void SetNeighborMineCount(int count) {
        neighborMineCount = count;
    }

    public void Open(BlockInfo.State state) {
        open = true;
        currentState = state;
    }

    public void Close() {
        open = false;
        currentState = BlockInfo.State.None;
    }

    public void CycleThroughStates() {
        switch (currentState) {
            case State.None:
            case State.Count:
            case State.Mine:
                currentState = State.Flag;
                break;
            case State.Flag:
                currentState = State.Cross;
                break;
            case State.Cross:
                currentState = State.None;
                break;
        }
    }

    public BlockInfo.State GetCurrentState() {
        return currentState;
    }
}
