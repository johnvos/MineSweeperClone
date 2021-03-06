﻿using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject BlockInfoPrefab;
    public GameObject BoundaryBlockPrefab;

    List<Block> gameBoard;
    List<BlockInfo> infoBoard;

    List<BlockInfo.State> fullInfo;

    int width;
    int height;
    int MineCount;

    bool start;
    double scale;

    int gameState; // -2: not spawned, -1: not intialized, 0: game not done, 1: game won, 99: mine exploded

    public void SetupBoard(int width, int height, int MineCount) {
        this.width = width;
        this.height = height;
        this.MineCount = MineCount;

        if (MineCount >= width * height) {
            throw new System.Exception("MineCount is too high.\nCurrent width: " + width + "\nCurrent height: " + height + "\nMax minecount: " + width * height);
        }

        start = false;
        gameState = -2;

        gameBoard = new List<Block>(width * height);
        infoBoard = new List<BlockInfo>(width * height);
        fullInfo = new List<BlockInfo.State>(width * height);
    }

    public void SpawnBoard() {
        
        Vector3 zerozero = new Vector3(-width/2f, height/2f, 0);

        if (width % 2 == 0) {
            zerozero += new Vector3(0.5f, 0, 0);
        }

        if (height % 2 == 0) {
            zerozero += new Vector3(0, -0.5f, 0);
        }

        for (int j = 0; j < height; j++) {
            for(int i = 0; i < width; i++) {
                //spawning inner blocks
                Vector3 offset = new Vector3(i, -j, 0);
                Vector3 infoOffset = new Vector3(i, -j, -1);

                Vector3 spawnPoint = zerozero + offset;
                Vector3 infoSpawnPoint = zerozero + infoOffset;

                Block block = Instantiate(BlockPrefab, spawnPoint, Quaternion.identity).GetComponent<Block>();
                block.SetParent(this, i, j);

                gameBoard.Add(block);

                BlockInfo blockInfo = Instantiate(BlockInfoPrefab, infoSpawnPoint, Quaternion.identity).GetComponent<BlockInfo>();

                infoBoard.Add(blockInfo);

                fullInfo.Add(BlockInfo.State.None);

                //spawning outer boundaries
                
                //left boundaries
                if(i == 0) {
                    Vector3 leftOffset = new Vector3(-1, -j);

                    Instantiate(BoundaryBlockPrefab, zerozero + leftOffset, Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(4);
                }

                //right boundaries
                if(i == width - 1) {
                    Vector3 rightOffset = new Vector3(width, -j);

                    Instantiate(BoundaryBlockPrefab, zerozero + rightOffset, Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(6);
                }

                //top boundaries
                if(j == 0) {
                    Vector3 topOffset = new Vector3(i, 1);

                    Instantiate(BoundaryBlockPrefab, zerozero + topOffset, Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(8);
                }

                //bottom boundaries
                if(j == height - 1) {
                    Vector3 bottomOffset = new Vector3(i, -height);

                    Instantiate(BoundaryBlockPrefab, zerozero + bottomOffset, Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(2);
                }

                //TL
                Instantiate(BoundaryBlockPrefab, zerozero + new Vector3(-1,1), Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(7);
                //TR
                Instantiate(BoundaryBlockPrefab, zerozero + new Vector3(width, 1), Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(9);
                //BL
                Instantiate(BoundaryBlockPrefab, zerozero + new Vector3(-1, -height), Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(1);
                //BR
                Instantiate(BoundaryBlockPrefab, zerozero + new Vector3(width, -height), Quaternion.identity).GetComponent<BoundaryBlock>().SetPosition(3);

            }
        }


        gameState = -1;
    }

    public void ClickLeft(int x, int y) {
        if (!start) { //this is the first move
            // Initiate the board mines and all that stuff
            // EXCLUDING this particular block
            InitializeBoard(x, y);
        } else {
            Open(gameBoard[CoordToIndex(x,y)]);
        }
    }

    public void ClickRight(int x, int y) {
        if (gameBoard[CoordToIndex(x, y)].IsOpen()) return;

        infoBoard[CoordToIndex(x, y)].CycleThroughStates();
    }

    public int GetCurrentGameState() {
        return gameState;
    }

    //---------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------

    private int CoordToIndex(int x, int y) {
        return (width * y + x);
    }

    private List<int> GetNeighboringIndices(int index) {
        List<int> neighbors = new List<int>();
        int right = index + 1;
        if (right < width * height && right % width != 0) neighbors.Add(right);

        int left = index - 1;
        if (left >= 0 && left % width != width - 1) neighbors.Add(left);

        int top = index - width;
        if (top >= 0) {
            neighbors.Add(top);
        }

        int bottom = index + width;
        if (bottom < width * height) {
            neighbors.Add(bottom);
        }

        //string debugging = "";
        //foreach(int x in neighbors) {
        //    debugging += x.ToString() + " ";
        //}
        //Debug.Log("For index " + index + ": " + debugging);

        return neighbors;
    }

    private List<int> GetSurroundingIndices(int index) {
        List<int> neighbors = new List<int>();
        int right = index + 1;
        if (right < width * height && right%width!=0) neighbors.Add(right);
        
        int left = index - 1;
        if (left >= 0 && left%width!=width-1) neighbors.Add(left);

        int top = index - width;
        if (top >= 0) {
            neighbors.Add(top);

            int tr = top + 1;
            if (tr % width != 0) neighbors.Add(tr);

            int tl = top - 1;
            if (tl % width != width - 1 && tl>=0) neighbors.Add(tl);
        }
        
        int bottom = index + width;
        if (bottom < width * height) { 
            neighbors.Add(bottom);

            int br = bottom + 1;
            if (br % width != 0) neighbors.Add(br);

            int bl = bottom - 1;
            if (bl % width != width - 1) neighbors.Add(bl);
        }

        //string debugging = "";
        //foreach(int x in neighbors) {
        //    debugging += x.ToString() + " ";
        //}
        //Debug.Log("For index " + index + ": " + debugging);

        return neighbors;
    }

    private void InitializeBoard(int x, int y) { //intialize, with (x,y) guaranteed no-mine
        Block block = gameBoard[CoordToIndex(x, y)];
        int clicked = CoordToIndex(block.GetX(), block.GetY());

        if (infoBoard[clicked].GetCurrentState() == BlockInfo.State.Cross
            || infoBoard[clicked].GetCurrentState() == BlockInfo.State.Flag) {
            return; //because we don't wanna open crossed or flagged
        }

        start = true;

        System.Random rand = new System.Random();
        //Initialize Mine
        for(int i = 0; i < MineCount; i++) {
            int randIndex = -1;
            do {
                randIndex = rand.Next(width * height);
                //Debug.Log("Random: " + randIndex);
            } while (fullInfo[randIndex] == BlockInfo.State.Mine || randIndex == CoordToIndex(x,y));
            fullInfo[randIndex] = BlockInfo.State.Mine;
            infoBoard[randIndex].SetInfo(BlockInfo.State.Mine);
        }

        //Count neighbor mines
        for(int i = 0; i < width * height; i++) {
            if (fullInfo[i] == BlockInfo.State.Mine) continue;

            int surroundingMineCount = 0;
            List<int> surroundingIndices = GetSurroundingIndices(i);
            foreach(int surroundingIndex in surroundingIndices) {
                if (fullInfo[surroundingIndex] == BlockInfo.State.Mine) surroundingMineCount++;
            }
            if(surroundingMineCount != 0) {
                fullInfo[i] = BlockInfo.State.Count;
                infoBoard[i].SetInfo(BlockInfo.State.Count);
                infoBoard[i].SetNeighborMineCount(surroundingMineCount);
            }
        }

        Open(gameBoard[CoordToIndex(x,y)]);
        gameState = 0;
    }


    private void Open(Block block) {
        int clicked = CoordToIndex(block.GetX(), block.GetY());

        if(infoBoard[clicked].GetCurrentState()==BlockInfo.State.Cross 
            || infoBoard[clicked].GetCurrentState()==BlockInfo.State.Flag) {
            return; //because we don't wanna open crossed or flagged
        }

        gameBoard[clicked].Open();
        infoBoard[clicked].Open(fullInfo[clicked]);

        if(fullInfo[clicked] == BlockInfo.State.Mine) {
            Debug.Log("MINE!!! YOU DEAD BRO");
            gameState = 99;
            return;
        }

        if(fullInfo[clicked] != BlockInfo.State.Count) {
            List<int> openList = FindAllOpenableNeighbors(clicked);
            foreach (int x in openList) {
                gameBoard[x].Open();
                infoBoard[x].Open(fullInfo[x]);
            }
        }

        CheckBoard();
    }

    private List<int> FindAllOpenableNeighbors(int index) {
        List<int> list = new List<int>();
        Debug.Log(list.Count);
        List<int> neighbors = GetNeighboringIndices(index);
        foreach(int x in neighbors) {
            Recurse(x, ref list);
        }
        Debug.Log(list.Count);
        return list;
    }

    private void Recurse(int index, ref List<int> list) {
        if (list.Contains(index)) return;

        if (!gameBoard[index].IsOpen()
            && (fullInfo[index] == BlockInfo.State.Count || fullInfo[index] == BlockInfo.State.None)
            && (infoBoard[index].GetCurrentState() != BlockInfo.State.Flag && infoBoard[index].GetCurrentState() != BlockInfo.State.Cross)) {
            list.Add(index);
            Debug.Log("Just added a block of state " + infoBoard[index].GetCurrentState());
            if (fullInfo[index] == BlockInfo.State.None) {
                List<int> neighbors = GetNeighboringIndices(index);
                foreach (int x in neighbors) {
                    Recurse(x, ref list);
                }
            }

        }
    }

    private void CheckBoard() { // Checks if all mines are found
        int unopenedBlocks = 0;
        foreach(Block x in gameBoard) {
            if (!x.IsOpen()) unopenedBlocks++;
        }
        if (unopenedBlocks == MineCount) {
            gameState = 1;
            Debug.Log("Hey I think you won");
        }
        Debug.Log("Unopened Blocks: " + unopenedBlocks);
    }

}
