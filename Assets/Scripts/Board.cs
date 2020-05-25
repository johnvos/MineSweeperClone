using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject BlockInfoPrefab;

    Camera mainCam;
    List<Block> gameBoard;
    List<BlockInfo> infoBoard;

    List<BlockInfo.State> fullInfo;

    public int width = 10;
    public int height = 10;
    public int MineCount = 10;

    bool start;
    double scale;

    private void Awake() {
        start = false;

        mainCam = Camera.main;

        gameBoard = new List<Block>(width * height);
        infoBoard = new List<BlockInfo>(width * height);
        fullInfo = new List<BlockInfo.State>(width * height);
    }


    // Start is called before the first frame update
    void Start()
    {
        SpawnBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBoard() {
        
        Vector3 zerozero = new Vector3(-width/2f, height/2f, 0);

        if (width % 2 == 0) {
            zerozero += new Vector3(0.5f, 0, 0);
        }

        if (height % 2 == 0) {
            zerozero += new Vector3(0, -0.5f, 0);
        }


        for (int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
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
            }
        }
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

    }

    private int CoordToIndex(int x, int y) {
        return (width * y + x);
    }

    private List<int> GetNeighboringIndices(int index) {
        List<int> neighbors = new List<int>();
        int right = index + 1;
        if (right < width && right < width * height && right % width != 0) neighbors.Add(right);

        int left = index - 1;
        if (left > 0 && left % width != width - 1) neighbors.Add(left);

        int top = index - width;
        if (top > 0) {
            neighbors.Add(top);
        }

        int bottom = index + width;
        if (bottom < width * height) {
            neighbors.Add(bottom);
        }

        return neighbors;
    }

    private List<int> GetSurroundingIndices(int index) {
        List<int> neighbors = new List<int>();
        int right = index + 1;
        if (right < width && right < width * height && right%width!=0) neighbors.Add(right);
        
        int left = index - 1;
        if (left > 0 && left%width!=width-1) neighbors.Add(left);

        int top = index - width;
        if (top > 0) {
            neighbors.Add(top);

            int tr = top + 1;
            if (tr % width != 0) neighbors.Add(tr);

            int tl = top - 1;
            if (tl % width != width - 1) neighbors.Add(tl);
        }
        
        int bottom = index + width;
        if (bottom < width * height) { 
            neighbors.Add(bottom);

            int br = bottom + 1;
            if (br % width != 0) neighbors.Add(br);

            int bl = bottom - 1;
            if (bl % width != width - 1) neighbors.Add(bl);
        }

        return neighbors;
    }

    private void InitializeBoard(int x, int y) { //intialize, with (x,y) guaranteed no-mine
        start = true;

        System.Random rand = new System.Random();
        //Initialize Mine
        for(int i = 0; i <= MineCount; i++) {
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
        
    }


    private void Open(Block block) {
        int clicked = CoordToIndex(block.GetX(), block.GetY());

        if(fullInfo[clicked]==BlockInfo.State.Cross 
            || fullInfo[clicked]==BlockInfo.State.Flag) {
            return; //because we don't wanna open crossed or flagged
        }

        gameBoard[clicked].Open();
        infoBoard[clicked].Open();

        if(fullInfo[clicked] == BlockInfo.State.Mine) {
            Debug.Log("MINE!!! YOU DEAD BRO");
            return;
        }


        //TODO: this is causing infinite loop.
        // Probably because I'm checking already opened blocks as well
        // gosh this is too cumbersome but what choices do I have
        // hehe
        List<int> openList = FindAllOpenableNeighbors(clicked);
        foreach(int x in openList) {
            gameBoard[x].Open();
            infoBoard[x].Open();
        }
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

        if(!gameBoard[index].IsOpen() && (fullInfo[index] == BlockInfo.State.Count || fullInfo[index] == BlockInfo.State.None)) {
            list.Add(index);
            List<int> neighbors = GetNeighboringIndices(index);
            foreach(int x in neighbors) {
                Recurse(x, ref list);
            }
        }
    }

}
