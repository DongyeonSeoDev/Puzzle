using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform blockPositionImages = null;
    [SerializeField] private Transform blockObject = null;

    [Header("순서: eBlockType 순서대로")]
    [SerializeField] private List<GameObject> blockList = new List<GameObject>();
    [SerializeField] private List<Block> blockPool = new List<Block>();

    [SerializeField] private Sprite[] animalSprites = null;
    [SerializeField] private Texture2D[] animalTexture = null;

    private Transform[][] blockPositions = new Transform[9][];
    private List<List<Block>> blocks = new List<List<Block>>();

    public static Map instance;

    private int blockX = 0;
    private int blockY = 0;

    private Block currentBlock = null;

    private Vector2 buttonDown = Vector2.zero;
    private Vector2 buttonUp = Vector2.zero;

    private float moveX = 0f;
    private float moveY = 0f;

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

        CheckBlock(true, false);
    }

    private void Start()
    {
        foreach (var barricade in GamePlayManager.Instance.barricade[GamePlayManager.Instance.stageNumber].position)
        {
            blocks[barricade.x][barricade.y].gameObject.SetActive(false);
            blocks[barricade.x][barricade.y] = CreateBlock((int)eBlockType.BARRICADE);
            blocks[barricade.x][barricade.y].transform.position = blockPositions[barricade.x][barricade.y].position;
            blocks[barricade.x][barricade.y].gameObject.SetActive(true);
        }

        if (GamePlayManager.Instance.stageNumber >= 2)
        {
            Vector2Int animalPos = GamePlayManager.Instance.animalPos[GamePlayManager.Instance.stageNumber];
            Vector2Int animal2Pos = GamePlayManager.Instance.animal2Pos[GamePlayManager.Instance.stageNumber];

            blocks[animalPos.x][animalPos.y].gameObject.SetActive(false);
            blocks[animalPos.x][animalPos.y] = CreateBlock((int)eBlockType.ANIMALS);
            blocks[animalPos.x][animalPos.y].transform.position = blockPositions[animalPos.x][animalPos.y].position;
            blocks[animalPos.x][animalPos.y].image.sprite = animalSprites[GamePlayManager.Instance.stageNumber * 2];
            blocks[animalPos.x][animalPos.y].effectTexture = animalTexture[GamePlayManager.Instance.stageNumber * 2];
            blocks[animalPos.x][animalPos.y].gameObject.SetActive(true);

            blocks[animal2Pos.x][animal2Pos.y].gameObject.SetActive(false);
            blocks[animal2Pos.x][animal2Pos.y] = CreateBlock((int)eBlockType.ANIMALS);
            blocks[animal2Pos.x][animal2Pos.y].transform.position = blockPositions[animal2Pos.x][animal2Pos.y].position;
            blocks[animalPos.x][animalPos.y].image.sprite = animalSprites[GamePlayManager.Instance.stageNumber * 2 + 1];
            blocks[animalPos.x][animalPos.y].effectTexture = animalTexture[GamePlayManager.Instance.stageNumber * 2 + 1];
            blocks[animal2Pos.x][animal2Pos.y].gameObject.SetActive(true);
        }

        GamePlayManager.Instance.gameClearEvent += () =>
        {
            StartCoroutine(CheckSpecialBlock());
        };
    }

    private void Update()
    {
        if (GamePlayManager.Instance.gameEnd)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            buttonDown = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            UpdateBlock();
        }
    }

    private void UpdateBlock(bool gameClear = false)
    {
        if (currentBlock == null || currentBlock.blockType == eBlockType.BARRICADE)
        {
            currentBlock = null;
            return;
        }

        if (BlockTypeCheck(currentBlock, eBlockType.LEAF))
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[blockX][i].blockType != eBlockType.ANIMALS)
                {
                    blocks[blockX][i].gameObject.SetActive(false);

                    GamePlayManager.Instance.BlockBroken(blocks[blockX][i], 1);
                    GamePlayManager.Instance.BlockParticle(blocks[blockX][i]);
                }

                if (blocks[i][blockY].blockType != eBlockType.ANIMALS)
                {
                    blocks[i][blockY].gameObject.SetActive(false);

                    GamePlayManager.Instance.BlockBroken(blocks[i][blockY], 1);
                    GamePlayManager.Instance.BlockParticle(blocks[i][blockY]);
                }
            }
        }
        else if (BlockTypeCheck(currentBlock, eBlockType.BOMB))
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (blockX - 2 + i >= 0 && blockX - 2 + i < blocks.Count && blockY - 2 + j >= 0 && blockY - 2 + j < blocks[0].Count)
                    {
                        if (blocks[blockX - 2 + i][blockY - 2 + j].blockType != eBlockType.ANIMALS)
                        {
                            blocks[blockX - 2 + i][blockY - 2 + j].gameObject.SetActive(false);

                            GamePlayManager.Instance.BlockBroken(blocks[blockX - 2 + i][blockY - 2 + j], 1);
                            GamePlayManager.Instance.BlockParticle(blocks[blockX - 2 + i][blockY - 2 + j]);
                        }
                    }
                }
            }
        }
        else if (BlockTypeCheck(currentBlock, eBlockType.GIFTBOX))
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (blockX - 1 + i >= 0 && blockX - 1 + i < blocks.Count && blockY - 1 + j >= 0 && blockY - 1 + j < blocks[0].Count)
                    {
                        if (blocks[blockX - 1 + i][blockY - 1 + j].blockType != eBlockType.ANIMALS)
                        {
                            blocks[blockX - 1 + i][blockY - 1 + j].gameObject.SetActive(false);

                            GamePlayManager.Instance.BlockBroken(blocks[blockX - 1 + i][blockY - 1 + j], 1);
                            GamePlayManager.Instance.BlockParticle(blocks[blockX - 1 + i][blockY - 1 + j]);
                        } 
                    }
                }
            }
        }
        else if (BlockTypeCheck(currentBlock, eBlockType.ANIMALS))
        {
            if (blockX == 0 || blockY == 0 || blockX == 8 || blockY == 8)
            {
                currentBlock.gameObject.SetActive(false);
                GamePlayManager.Instance.BlockBroken(currentBlock, 15);
                GamePlayManager.Instance.BlockParticle(currentBlock);
            }
        }
        else
        {
            buttonUp = Input.mousePosition;

            moveX = buttonDown.x - buttonUp.x;
            moveY = buttonDown.y - buttonUp.y;

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

            if (BlockTypeCheck(currentBlock, eBlockType.UNICORN))
            {
                eBlockType eventBlockType = blocks[blockX + (int)moveY][blockY + (int)moveX].blockType;
                int breakBlockCount = 0;

                for (int i = 0; i < blocks.Count; i++)
                {
                    for (int j = 0; j < blocks[0].Count; j++)
                    {
                        if (blocks[i][j].blockType == eventBlockType)
                        {
                            blocks[i][j].gameObject.SetActive(false);
                            breakBlockCount++;

                            GamePlayManager.Instance.BlockParticle(blocks[i][j]);
                        }
                    }
                }

                if (blocks[blockX + (int)moveY][blockY + (int)moveX].blockType == eBlockType.BARRICADE)
                {
                    currentBlock = null;
                    return;
                }

                blocks[blockX][blockY].gameObject.SetActive(false);

                GamePlayManager.Instance.BlockBroken(blocks[blockX + (int)moveY][blockY + (int)moveX], breakBlockCount);
                GamePlayManager.Instance.BlockBroken(blocks[blockX][blockY], 1);

                GamePlayManager.Instance.BlockParticle(blocks[blockX][blockY]);
            }
            else
            {
                var tempBlock = blocks[blockX + (int)moveY][blockY + (int)moveX];
                blocks[blockX + (int)moveY][blockY + (int)moveX] = currentBlock;
                blocks[blockX][blockY] = tempBlock;

                if (CheckBlock(false, true))
                {
                    return;
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
                            if (blocks[i - 1][j].blockType == eBlockType.BARRICADE || blocks[i][j].blockType == eBlockType.BARRICADE)
                            {
                                continue;
                            }

                            isChange = true;

                            var tempBlock = blocks[i - 1][j];
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
                        int random = Random.Range(0, 100);
                        Block block;

                        if (random == 1)
                        {
                            block = CreateBlock((int)eBlockType.GIFTBOX);
                        }
                        else
                        {
                            block = CreateBlock(Random.Range(0, 5));
                        }

                        block.transform.position = blockPositions[i][j].position;
                        block.gameObject.SetActive(true);
                        blocks[i][j] = block;
                    }
                }
            }

            if (CheckBlock(false, false))
            {
                GamePlayManager.Instance.MoveBlock(gameClear);
                currentBlock = null;
                break;
            }
        } //end of while
    }

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
            blockPool.Add(Instantiate(blockList[type], blockObject).GetComponent<Block>());
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

    private bool CheckBlock(bool isFirst, bool isChangeBlock)
    {
        bool isChange = true;

        bool isFirstCheck = true;
        int posI = 0;
        int posJ = 0;
        bool up = true;

        while (true)
        {
            isChange = false;

            if (isFirst)
            {
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
            }
            else
            {
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
                            posI = i;
                            posJ = j + 1;
                            up = false;
                            isChange = true;

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
                                posI = i + 1;
                                posJ = j;
                                isChange = true;

                                i = 7;
                                j = 9;

                                break;
                            }
                        }
                    }
                }
            }

            if (isFirst)
            {
                if (!isChange)
                {
                    break;
                }
            }
            else
            {
                if (isChangeBlock && !isChange && isFirstCheck)
                {
                    var tempBlock = blocks[blockX + (int)moveY][blockY + (int)moveX];
                    blocks[blockX + (int)moveY][blockY + (int)moveX] = blocks[blockX][blockY];
                    blocks[blockX][blockY] = tempBlock;
                }

                if (!isChange)
                {
                    if (isChangeBlock)
                    {
                        currentBlock = null;
                    }

                    break;
                }
                else
                {
                    isFirstCheck = false;

                    if (isChangeBlock)
                    {
                        blocks[blockX][blockY].transform.position = blockPositions[blockX][blockY].position;
                        blocks[blockX + (int)moveY][blockY + (int)moveX].transform.position = blockPositions[blockX + (int)moveY][blockY + (int)moveX].position;
                    }

                    var blockType = blocks[posI][posJ].blockType;
                    int blockCount = 0;

                    eBlockType? specialBlockType = null;

                    if (up)
                    {
                        if (BlockCheck(posI, posJ, up))
                        {
                            blockCount += 2;
                            specialBlockType = eBlockType.BOMB;
                        }
                        else
                        {
                            if (posI + 2 < blocks.Count && blockType == blocks[posI + 2][posJ].blockType) // 4개가 같은지 확인
                            {
                                if (posI + 3 < blocks.Count && blockType == blocks[posI + 3][posJ].blockType) //5개가 같은지 확인
                                {
                                    blocks[posI + 3][posJ].gameObject.SetActive(false);
                                    GamePlayManager.Instance.BlockParticle(blocks[posI + 3][posJ]); //5번째 블록 이펙트
                                    blockCount++;

                                    specialBlockType = eBlockType.UNICORN;
                                }
                                else
                                {
                                    specialBlockType = eBlockType.LEAF;
                                }

                                blocks[posI + 2][posJ].gameObject.SetActive(false); //4번째 블록 이펙트
                                GamePlayManager.Instance.BlockParticle(blocks[posI + 2][posJ]);
                                blockCount++;
                            }
                        }

                        BlockBreak(posI, posJ, up);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], blockCount + 3);
                    }
                    else
                    {
                        if (BlockCheck(posI, posJ, up))
                        {
                            blockCount += 2;
                            specialBlockType = eBlockType.BOMB;
                        }
                        else
                        {
                            if (posJ + 2 < blocks[0].Count && blockType == blocks[posI][posJ + 2].blockType)  // 4개가 같은지 확인
                            {
                                if (posJ + 3 < blocks[0].Count && blockType == blocks[posI][posJ + 3].blockType) // 5개가 같은지 확인
                                {
                                    blocks[posI][posJ + 3].gameObject.SetActive(false);
                                    GamePlayManager.Instance.BlockParticle(blocks[posI][posJ + 3]); //5번째 블록 이펙트
                                    blockCount++;

                                    specialBlockType = eBlockType.UNICORN;
                                }
                                else
                                {
                                    specialBlockType = eBlockType.LEAF;
                                }

                                blocks[posI][posJ + 2].gameObject.SetActive(false);
                                blocks[posI][posJ + 2].gameObject.SetActive(false); //4번째 블록 이펙트
                                blockCount++;
                            }
                        }

                        BlockBreak(posI, posJ, up);

                        GamePlayManager.Instance.BlockBroken(blocks[posI][posJ], blockCount + 3);
                    }

                    if (!(specialBlockType is null))
                    {
                        blocks[posI][posJ] = CreateBlock((int)specialBlockType);
                        blocks[posI][posJ].transform.position = blockPositions[posI][posJ].position;
                        blocks[posI][posJ].gameObject.SetActive(true);
                    }
                }
            }
        }

        return isFirstCheck;
    }

    private void BlockBreak(int middleX, int middleY, bool isUp)
    {
        if (isUp)
        {
            blocks[middleX - 1][middleY].gameObject.SetActive(false);
            blocks[middleX][middleY].gameObject.SetActive(false);
            blocks[middleX + 1][middleY].gameObject.SetActive(false);

            GamePlayManager.Instance.BlockParticle(blocks[middleX - 1][middleY]); //1번째 블록 이펙트
            GamePlayManager.Instance.BlockParticle(blocks[middleX][middleY]); //2번째 블록 이펙트
            GamePlayManager.Instance.BlockParticle(blocks[middleX + 1][middleY]); //3번째 블록 이펙트
        }
        else
        {
            blocks[middleX][middleY - 1].gameObject.SetActive(false);
            blocks[middleX][middleY].gameObject.SetActive(false);
            blocks[middleX][middleY + 1].gameObject.SetActive(false);

            GamePlayManager.Instance.BlockParticle(blocks[middleX][middleY - 1]); //1번째 블록 이펙트
            GamePlayManager.Instance.BlockParticle(blocks[middleX][middleY]); //2번째 블록 이펙트
            GamePlayManager.Instance.BlockParticle(blocks[middleX][middleY + 1]); //3번째 블록 이펙트
        }
    }

    private bool BlockCheck(int middleX, int middleY, bool isUp)
    {
        int[] posX = new int[2];
        int[] posY = new int[2];

        eBlockType type = blocks[middleX][middleY].blockType;

        if (isUp)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (BlockTypeCheck(middleX + i - 1, middleY - 2 + j, type) && BlockTypeCheck(middleX + i - 1, middleY - 1 + j, type) && BlockTypeCheck(middleX + i - 1, middleY + j, type))
                    {
                        posX[0] = middleX + i - 1;
                        posX[1] = middleX + i - 1;

                        switch (j)
                        {
                            case 0:
                                posY[0] = middleY - 2;
                                posY[1] = middleY - 1;
                                break;
                            case 1:
                                posY[0] = middleY - 1;
                                posY[1] = middleY + 1;
                                break;
                            case 2:
                                posY[0] = middleY + 1;
                                posY[1] = middleY + 2;
                                break;
                        }

                        BlockCheckTrue(posX, posY);

                        return true;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (BlockTypeCheck(middleX - 2 + j, middleY + i - 1, type) && BlockTypeCheck(middleX - 1 + j, middleY + i - 1, type) && BlockTypeCheck(middleX + j, middleY + i - 1, type))
                    {
                        posY[0] = middleY + i - 1;
                        posY[1] = middleY + i - 1;

                        switch (j)
                        {
                            case 0:
                                posX[0] = middleX - 2;
                                posX[1] = middleX - 1;
                                break;          
                            case 1:             
                                posX[0] = middleX - 1;
                                posX[1] = middleX + 1;
                                break;          
                            case 2:             
                                posX[0] = middleX + 1;
                                posX[1] = middleX + 2;
                                break;
                        }

                        BlockCheckTrue(posX, posY);

                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool BlockTypeCheck(int posX, int posY, eBlockType type)
    {
        if (posX < 0 || posY < 0 || posX >= blocks.Count || posY >= blocks[0].Count)
        {
            return false;
        }

        return blocks[posX][posY].blockType == type;
    }

    private bool BlockTypeCheck(Block block, eBlockType type)
    {
        if (block is null)
        {
            return false;
        }

        return block.blockType == type;
    }

    private void BlockCheckTrue(int[] posX, int[] posY)
    {
        for (int i = 0; i < 2; i++)
        {
            blocks[posX[i]][posY[i]].gameObject.SetActive(false);

            GamePlayManager.Instance.BlockParticle(blocks[posX[i]][posY[i]]);
        }
    }

    private IEnumerator CheckSpecialBlock()
    {
        bool isChange = true;

        while (isChange)
        {
            isChange = false;

            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[0].Count; j++)
                {
                    if (BlockTypeCheck(i, j, eBlockType.UNICORN) || BlockTypeCheck(i, j, eBlockType.BOMB) || BlockTypeCheck(i, j, eBlockType.LEAF) || BlockTypeCheck(i, j, eBlockType.GIFTBOX))
                    {
                        Move(blocks[i][j]);
                        UpdateBlock(true);
                        
                        isChange = true;

                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        yield return new WaitForSeconds(1.5f);

        SetGiftBox();

        yield return new WaitForSeconds(1.5f);

        isChange = true;

        while (isChange)
        {
            isChange = false;

            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[0].Count; j++)
                {
                    if (BlockTypeCheck(i, j, eBlockType.UNICORN) || BlockTypeCheck(i, j, eBlockType.BOMB) || BlockTypeCheck(i, j, eBlockType.LEAF) || BlockTypeCheck(i, j, eBlockType.GIFTBOX))
                    {
                        Move(blocks[i][j]);
                        UpdateBlock();

                        isChange = true;

                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        GamePlayManager.Instance.GameClearPanel();
    }

    private void SetGiftBox()
    {
        int i = 0;
        int j = 0;
        bool check = false;
        int count = 0;

        while (GamePlayManager.Instance.limitCountCheck())
        {
            count++;

            if (count > 50)
            {
                return;
            }

            check = false;

            while (!check)
            {
                check = true;

                i = Random.Range(0, blocks.Count);
                j = Random.Range(0, blocks[0].Count);
                
                if (blocks[i][j].blockType == eBlockType.GIFTBOX)
                {
                    check = false;
                }
            }

            blocks[i][j].gameObject.SetActive(false);
            blocks[i][j] = CreateBlock((int)eBlockType.GIFTBOX);
            blocks[i][j].transform.position = blockPositions[i][j].position;
            blocks[i][j].gameObject.SetActive(true);
        }
    }
}
