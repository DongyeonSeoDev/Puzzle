using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform blockPositionImages = null;
    [SerializeField] private Transform bulletObject = null;

    [Header("(eBlockType 순서: red, blue, purple, orange, green)")]
    [Header("eBlockType 순서대로 넣어주어야 합니다.")]
    
    [SerializeField] private List<GameObject> blockList = new List<GameObject>();
    [SerializeField] private List<Block> blockPool = new List<Block>();

    private Transform[][] blockPositions = new Transform[9][];
    private List<List<Block>> blocks = new List<List<Block>>();

    public static Map instance;

    private int blockX = 0;
    private int blockY = 0;

    private Block currentBlock = null;

    private Vector2 buttonDown = Vector2.zero;
    private Vector2 buttonUp = Vector2.zero;

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 9; i++)
        {
            Transform blockPosition = blockPositionImages.GetChild(i);
            blockPositions[i] = new Transform[9];

            for (int j = 0; j < 9; j++)
            {
                blockPositions[i][j] = blockPosition.GetChild(j);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            List<Block> block = new List<Block>();

            for (int j = 0; j < 9; j++)
            {
                block.Add(CreateBlock(Random.Range(0, 5)));
                block[j].gameObject.SetActive(true);
                block[j].gameObject.transform.position = blockPositions[i][j].position;
            }

            blocks.Add(block);
        }

        bool isChange = true;

        while (true)
        {
            isChange = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (blocks[i][j].blockType == blocks[i][j + 1].blockType && blocks[i][j + 1].blockType == blocks[i][j + 2].blockType)
                    {
                        int randomNum;

                        do
                        {
                            randomNum = Random.Range(0, 5);
                        }
                        while (blocks[i][j].blockType == (eBlockType)randomNum);

                        blocks[i][j].gameObject.SetActive(false);

                        Block block = CreateBlock(randomNum);
                        block.gameObject.SetActive(true);
                        block.gameObject.transform.position = blockPositions[i][j].position;

                        blocks[i][j] = block;
                        isChange = true;
                    }
                }
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (blocks[i][j].blockType == blocks[i + 1][j].blockType && blocks[i + 1][j].blockType == blocks[i + 2][j].blockType)
                    {
                        int randomNum;

                        do
                        {
                            randomNum = Random.Range(0, 5);
                        }
                        while (blocks[i][j].blockType == (eBlockType)randomNum);

                        blocks[i][j].gameObject.SetActive(false);

                        Block block = CreateBlock(randomNum);
                        block.gameObject.SetActive(true);
                        block.gameObject.transform.position = blockPositions[i][j].position;

                        blocks[i][j] = block;
                        isChange = true;
                    }
                }
            }

            if (!isChange)
            {
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonDown = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentBlock == null)
            {
                return;
            }

            buttonUp = Input.mousePosition;

            float moveX = buttonDown.x - buttonUp.x;
            float moveY = buttonDown.y - buttonUp.y;

            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {
                if (moveX > 0)
                {
                    moveX = -1;
                    moveY = 0;
                }
                else
                {
                    moveX = 1;
                    moveY = 0;
                }
            }
            else
            {
                if (moveY > 0)
                {
                    moveX = 0;
                    moveY = -1;
                }
                else
                {
                    moveX = 0;
                    moveY = 1;
                }
            }

            if ((blockX + (int)moveY) < 0 || (blockX + (int)moveY) > 8 || (blockY + (int)moveX) < 0 || (blockY + (int)moveX) > 8)
            {
                currentBlock = null;
                return;
            }

            Block temp = blocks[blockX + (int)moveY][blockY + (int)moveX];
            blocks[blockX + (int)moveY][blockY + (int)moveX] = currentBlock;
            blocks[blockX][blockY] = temp;

            bool check = false;
            bool isChenage = false;
            int posI = 0;
            int posJ = 0;
            bool up = true;

            while (true)
            {
                check = false;
                posI = 0;
                posJ = 0;
                up = true;

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (blocks[i][j].blockType == blocks[i][j + 1].blockType && blocks[i][j + 1].blockType == blocks[i][j + 2].blockType 
                            && blocks[i][j].gameObject.activeSelf && blocks[i][j + 1].gameObject.activeSelf && blocks[i][j + 2].gameObject.activeSelf)
                        {
                            check = true;
                            posI = i;
                            posJ = j + 1;
                            up = false;

                            i = 9;
                            j = 7;
                            break;
                        }
                    }
                }

                if (up)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (blocks[i][j].blockType == blocks[i + 1][j].blockType && blocks[i + 1][j].blockType == blocks[i + 2][j].blockType
                                && blocks[i][j].gameObject.activeSelf && blocks[i + 1][j].gameObject.activeSelf && blocks[i + 2][j].gameObject.activeSelf)
                            {
                                check = true;

                                posI = i + 1;
                                posJ = j;
                                up = true;

                                i = 7;
                                j = 9;
                                break;
                            }
                        }
                    }
                }

                if (!check && !isChenage)
                {
                    temp = blocks[blockX + (int)moveY][blockY + (int)moveX];
                    blocks[blockX + (int)moveY][blockY + (int)moveX] = blocks[blockX][blockY];
                    blocks[blockX][blockY] = temp;

                    return;
                }

                if (!check)
                {
                    currentBlock = null;
                    break;
                }
                else
                {
                    isChenage = true;

                    blocks[blockX][blockY].transform.position = blockPositions[blockX][blockY].position;
                    blocks[blockX + (int)moveY][blockY + (int)moveX].transform.position = blockPositions[blockX + (int)moveY][blockY + (int)moveX].position;

                    if (up)
                    {
                        blocks[posI - 1][posJ].gameObject.SetActive(false);
                        blocks[posI][posJ].gameObject.SetActive(false);
                        blocks[posI + 1][posJ].gameObject.SetActive(false);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], 3);
                    }
                    else
                    {
                        blocks[posI][posJ - 1].gameObject.SetActive(false);
                        blocks[posI][posJ].gameObject.SetActive(false);
                        blocks[posI][posJ + 1].gameObject.SetActive(false);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], 3);
                    }
                }
            }

            while (true)
            {
                bool isChange = true;

                while (isChange)
                {
                    isChange = false;

                    for (int i = 0; i < blocks.Count; i++)
                    {
                        for (int j = 0; j < blocks[i].Count; j++)
                        {
                            if (i - 1 >= 0 && !blocks[i - 1][j].gameObject.activeSelf && blocks[i][j].gameObject.activeSelf)
                            {
                                isChange = true;

                                Block tempBlock = blocks[i - 1][j];
                                blocks[i - 1][j] = blocks[i][j];
                                blocks[i][j] = tempBlock;

                                blocks[i][j].transform.position = blockPositions[i][j].position;
                                blocks[i - 1][j].transform.position = blockPositions[i - 1][j].position;
                            }
                        }
                    }
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    for (int j = 0; j < blocks[i].Count; j++)
                    {
                        if (blocks[i][j] == null || !blocks[i][j].gameObject.activeSelf)
                        {
                            Block block = CreateBlock(Random.Range(0, 5));
                            block.transform.position = blockPositions[i][j].position;
                            block.gameObject.SetActive(true);
                            blocks[i][j] = block;
                        }
                    }
                }

                bool isFirstCheck = true;

                while (true)
                {
                    check = false;
                    posI = 0;
                    posJ = 0;
                    up = true;

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (blocks[i][j].blockType == blocks[i][j + 1].blockType && blocks[i][j + 1].blockType == blocks[i][j + 2].blockType
                                && blocks[i][j].gameObject.activeSelf && blocks[i][j + 1].gameObject.activeSelf && blocks[i][j + 2].gameObject.activeSelf)
                            {
                                check = true;
                                posI = i;
                                posJ = j + 1;
                                up = false;

                                i = 9;
                                j = 7;
                                break;
                            }
                        }
                    }

                    if (up)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                if (blocks[i][j].blockType == blocks[i + 1][j].blockType && blocks[i + 1][j].blockType == blocks[i + 2][j].blockType
                                    && blocks[i][j].gameObject.activeSelf && blocks[i + 1][j].gameObject.activeSelf && blocks[i + 2][j].gameObject.activeSelf)
                                {
                                    check = true;

                                    posI = i + 1;
                                    posJ = j;
                                    up = true;

                                    i = 7;
                                    j = 9;
                                    break;
                                }
                            }
                        }
                    }

                    if (!check)
                    {
                        break;
                    }

                    isFirstCheck = false;

                    if (up)
                    {
                        blocks[posI - 1][posJ].gameObject.SetActive(false);
                        blocks[posI][posJ].gameObject.SetActive(false);
                        blocks[posI + 1][posJ].gameObject.SetActive(false);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], 3);
                    }
                    else
                    {
                        blocks[posI][posJ - 1].gameObject.SetActive(false);
                        blocks[posI][posJ].gameObject.SetActive(false);
                        blocks[posI][posJ + 1].gameObject.SetActive(false);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], 3);
                    }
                }

                if (isFirstCheck)
                {
                    GamePlayManager.Instance.MoveBlock();
                    break;
                }
            } //end of while
        } //end of if
    } //end of Update

    public static void Move(Block block)
    {
        int a, b;

        a = instance.blocks.FindIndex(x => x.Find(y => y == block));
        b = instance.blocks[a].FindIndex(x => x == block);

        instance.blockX = a;
        instance.blockY = b;
        instance.currentBlock = block;
    }

    private Block CreateBlock(int type)
    {
        Block block = blockPool.Find(x => !x.gameObject.activeSelf && x.blockType == (eBlockType)type);

        if (block == null)
        {
            blockPool.Add(Instantiate(blockList[type], bulletObject).GetComponent<Block>());
            blockPool[blockPool.Count - 1].gameObject.SetActive(false);

            return blockPool[blockPool.Count - 1];
        }

        int a, b;

        a = blocks.FindIndex(x => x.Find(y => y == block));


        if (a != -1)
        {
            b = blocks[a].FindIndex(x => x == block);

            if (b != -1)
            {
                blocks[a][b] = null;
            }
        }

        return block;
    }
}
